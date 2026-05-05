(function () {
    "use strict";

    var modal = document.getElementById("user-profile-modal");
    if (!modal) return;

    var maxPhotoBytes = 5 * 1024 * 1024;
    var openButtons = document.querySelectorAll(".js-user-profile-modal");
    var closeButtons = modal.querySelectorAll("[data-profile-close]");
    var statusBox = document.getElementById("user-profile-status");
    var form = document.getElementById("user-profile-form");
    var nameInput = document.getElementById("profile-name");
    var emailInput = document.getElementById("profile-email");
    var twoFactorInput = document.getElementById("profile-two-factor");
    var twoFactorConfirmPanel = document.getElementById("profile-two-factor-confirm");
    var twoFactorCodeInput = document.getElementById("profile-two-factor-code");
    var twoFactorConfirmButton = document.getElementById("profile-two-factor-confirm-btn");
    var photoInput = document.getElementById("profile-photo-input");
    var photoPick = document.getElementById("profile-photo-pick");
    var photoPreview = document.getElementById("profile-photo-preview");
    var photoInitials = document.getElementById("profile-photo-initials");
    var cropPanel = document.getElementById("profile-crop-panel");
    var cropCanvas = document.getElementById("profile-crop-canvas");
    var zoomInput = document.getElementById("profile-photo-zoom");
    var photoSave = document.getElementById("profile-photo-save");
    var passwordReset = document.getElementById("profile-password-reset");
    var cropContext = cropCanvas ? cropCanvas.getContext("2d") : null;
    var cropState = null;
    var dragState = null;
    var closeAfterSaveTimer = null;
    var pendingTwoFactorEnabled = null;

    function setStatus(message, type) {
        statusBox.textContent = message || "";
        statusBox.className = "user-profile-status";
        if (message) {
            statusBox.classList.add("is-visible", type === "error" ? "is-error" : "is-success");
        }
    }

    function setButtonBusy(button, busy, busyText) {
        if (!button) return;
        if (busy) {
            button.dataset.originalText = button.innerHTML;
            button.innerHTML = busyText;
            button.disabled = true;
            return;
        }

        if (button.dataset.originalText) {
            button.innerHTML = button.dataset.originalText;
            delete button.dataset.originalText;
        }
        button.disabled = false;
    }

    function getJson(response) {
        var contentType = response.headers.get("content-type") || "";
        if (contentType.indexOf("application/json") === -1) {
            return Promise.reject(new Error(response.redirected
                ? "Your session expired. Please sign in again."
                : "Unexpected server response. Please refresh the page."));
        }

        return response.json();
    }

    function applyProfile(data) {
        if (!data || typeof data.name !== "string" || typeof data.email !== "string") {
            throw new Error("Could not load profile data. Please refresh the page.");
        }

        nameInput.value = data.name || "";
        emailInput.value = data.email || "";
        twoFactorInput.checked = !!data.twoFactorEnabled;
        pendingTwoFactorEnabled = null;
        twoFactorConfirmPanel.hidden = true;
        twoFactorCodeInput.value = "";

        if (data.profileImagePath) {
            photoPreview.src = data.profileImagePath + "?v=" + Date.now();
            photoPreview.hidden = false;
            photoInitials.hidden = true;
        } else {
            photoPreview.removeAttribute("src");
            photoPreview.hidden = true;
            photoInitials.hidden = false;
            photoInitials.textContent = (data.name || "U").trim().charAt(0).toUpperCase();
        }
    }

    function openModal(event) {
        if (event) event.preventDefault();
        if (closeAfterSaveTimer) {
            window.clearTimeout(closeAfterSaveTimer);
            closeAfterSaveTimer = null;
        }
        modal.hidden = false;
        modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("user-profile-modal-open");
        setStatus("", "success");
        nameInput.focus();

        fetch("/Account/Profile", { headers: { "Accept": "application/json" } })
            .then(function (response) {
                return getJson(response).then(function (data) {
                    if (!response.ok) throw new Error(data.message || "Could not load profile.");
                    applyProfile(data);
                });
            })
            .catch(function (error) {
                setStatus(error.message, "error");
            });
    }

    function closeModal(event) {
        if (event) event.preventDefault();
        if (closeAfterSaveTimer) {
            window.clearTimeout(closeAfterSaveTimer);
            closeAfterSaveTimer = null;
        }
        modal.hidden = true;
        modal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("user-profile-modal-open");
    }

    function clampCrop() {
        if (!cropState) return;

        var canvasSize = cropCanvas.width;
        var drawWidth = cropState.image.width * cropState.scale;
        var drawHeight = cropState.image.height * cropState.scale;
        var maxX = Math.max(0, (drawWidth - canvasSize) / 2);
        var maxY = Math.max(0, (drawHeight - canvasSize) / 2);

        cropState.x = Math.min(maxX, Math.max(-maxX, cropState.x));
        cropState.y = Math.min(maxY, Math.max(-maxY, cropState.y));
    }

    function renderCrop() {
        if (!cropState || !cropContext) return;

        var canvasSize = cropCanvas.width;
        var image = cropState.image;
        var zoom = Number(zoomInput.value) / 100;
        cropState.scale = cropState.baseScale * zoom;
        clampCrop();

        var drawWidth = image.width * cropState.scale;
        var drawHeight = image.height * cropState.scale;
        var left = (canvasSize - drawWidth) / 2 + cropState.x;
        var top = (canvasSize - drawHeight) / 2 + cropState.y;

        cropContext.clearRect(0, 0, canvasSize, canvasSize);
        cropContext.fillStyle = "#ffffff";
        cropContext.fillRect(0, 0, canvasSize, canvasSize);
        cropContext.drawImage(image, left, top, drawWidth, drawHeight);
        cropContext.strokeStyle = "rgba(127, 175, 27, 0.85)";
        cropContext.lineWidth = 4;
        cropContext.strokeRect(2, 2, canvasSize - 4, canvasSize - 4);
    }

    function getPointerPosition(event) {
        var pointer = event.touches ? event.touches[0] : event;
        return { x: pointer.clientX, y: pointer.clientY };
    }

    function startCropDrag(event) {
        if (!cropState) return;
        var point = getPointerPosition(event);
        dragState = {
            x: point.x,
            y: point.y,
            cropX: cropState.x,
            cropY: cropState.y
        };
        event.preventDefault();
    }

    function moveCropDrag(event) {
        if (!cropState || !dragState) return;
        var point = getPointerPosition(event);
        cropState.x = dragState.cropX + point.x - dragState.x;
        cropState.y = dragState.cropY + point.y - dragState.y;
        renderCrop();
        event.preventDefault();
    }

    function stopCropDrag() {
        dragState = null;
    }

    function loadPhoto(file) {
        if (!file) return;

        if (file.size > maxPhotoBytes) {
            setStatus("Profile photo must be 5MB or smaller.", "error");
            photoInput.value = "";
            return;
        }

        if (!file.type || file.type.indexOf("image/") !== 0) {
            setStatus("Profile photo must be an image file.", "error");
            photoInput.value = "";
            return;
        }

        var objectUrl = URL.createObjectURL(file);
        var image = new Image();
        image.onload = function () {
            URL.revokeObjectURL(objectUrl);
            var canvasSize = cropCanvas.width;
            cropState = {
                image: image,
                baseScale: Math.max(canvasSize / image.width, canvasSize / image.height),
                scale: 1,
                x: 0,
                y: 0
            };
            zoomInput.value = "100";
            cropPanel.hidden = false;
            setStatus("Drag the image to frame a square crop, then save the photo.", "success");
            renderCrop();
        };
        image.onerror = function () {
            URL.revokeObjectURL(objectUrl);
            setStatus("Profile photo must be a valid image file.", "error");
        };
        image.src = objectUrl;
    }

    function savePhoto() {
        if (!cropState) {
            setStatus("Choose a profile photo first.", "error");
            return;
        }

        setButtonBusy(photoSave, true, "Saving...");
        cropCanvas.toBlob(function (blob) {
            if (!blob) {
                setButtonBusy(photoSave, false);
                setStatus("Could not prepare the profile photo.", "error");
                return;
            }

            var formData = new FormData();
            formData.append("photo", blob, "profile.webp");

            fetch("/Account/UploadProfilePhoto", {
                method: "POST",
                body: formData,
                headers: { "Accept": "application/json" }
            })
                .then(function (response) {
                    return getJson(response).then(function (data) {
                        if (!response.ok) throw new Error(data.message || "Could not save profile photo.");
                        photoPreview.src = data.profileImagePath + "?v=" + Date.now();
                        photoPreview.hidden = false;
                        photoInitials.hidden = true;
                        cropPanel.hidden = true;
                        cropState = null;
                        photoInput.value = "";
                        setStatus(data.message || "Profile photo saved.", "success");
                    });
                })
                .catch(function (error) {
                    setStatus(error.message, "error");
                })
                .finally(function () {
                    setButtonBusy(photoSave, false);
                });
        }, "image/webp", 0.86);
    }

    function saveProfile(event) {
        event.preventDefault();
        var submitButton = modal.querySelector(".user-profile-modal-footer .user-profile-primary-btn");
        var formData = new FormData();
        formData.append("name", nameInput.value);
        formData.append("twoFactorEnabled", twoFactorInput.checked ? "true" : "false");

        setButtonBusy(submitButton, true, "Saving...");

        fetch("/Account/Profile", {
            method: "POST",
            body: formData,
            headers: { "Accept": "application/json" }
        })
            .then(function (response) {
                return getJson(response).then(function (data) {
                    if (!response.ok) throw new Error(data.message || "Could not save profile.");
                    applyProfile(data);
                    setStatus(data.message || "Profile saved.", "success");

                    if (data.requiresTwoFactorConfirmation) {
                        pendingTwoFactorEnabled = !!data.pendingTwoFactorEnabled;
                        twoFactorInput.checked = pendingTwoFactorEnabled;
                        twoFactorConfirmPanel.hidden = false;
                        twoFactorCodeInput.focus();
                        return;
                    }

                    closeAfterSaveTimer = window.setTimeout(function () {
                        closeModal();
                    }, 700);
                });
            })
            .catch(function (error) {
                setStatus(error.message, "error");
            })
            .finally(function () {
                setButtonBusy(submitButton, false);
            });
    }

    function confirmTwoFactorSetting() {
        if (pendingTwoFactorEnabled === null) {
            setStatus("Save the two-factor change first.", "error");
            return;
        }

        var code = twoFactorCodeInput.value.trim();
        if (!/^\d{6}$/.test(code)) {
            setStatus("Enter the 6-digit email code.", "error");
            twoFactorCodeInput.focus();
            return;
        }

        var formData = new FormData();
        formData.append("code", code);
        formData.append("enabled", pendingTwoFactorEnabled ? "true" : "false");

        setButtonBusy(twoFactorConfirmButton, true, "Confirming...");

        fetch("/Account/ConfirmTwoFactorSetting", {
            method: "POST",
            body: formData,
            headers: { "Accept": "application/json" }
        })
            .then(function (response) {
                return getJson(response).then(function (data) {
                    if (!response.ok) throw new Error(data.message || "Could not confirm two-factor change.");
                    applyProfile(data);
                    pendingTwoFactorEnabled = null;
                    twoFactorConfirmPanel.hidden = true;
                    setStatus(data.message || "Two-factor setting updated.", "success");
                    closeAfterSaveTimer = window.setTimeout(function () {
                        closeModal();
                    }, 900);
                });
            })
            .catch(function (error) {
                setStatus(error.message, "error");
            })
            .finally(function () {
                setButtonBusy(twoFactorConfirmButton, false);
            });
    }

    function sendPasswordReset() {
        setButtonBusy(passwordReset, true, "Sending...");

        fetch("/Account/SendProfilePasswordReset", {
            method: "POST",
            headers: { "Accept": "application/json" }
        })
            .then(function (response) {
                return getJson(response).then(function (data) {
                    if (!response.ok) throw new Error(data.message || "Could not send reset link.");
                    setStatus(data.message || "Password reset link sent.", "success");
                });
            })
            .catch(function (error) {
                setStatus(error.message, "error");
            })
            .finally(function () {
                setButtonBusy(passwordReset, false);
            });
    }

    openButtons.forEach(function (button) {
        button.addEventListener("click", openModal);
    });

    closeButtons.forEach(function (button) {
        button.addEventListener("click", closeModal);
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && !modal.hidden) closeModal(event);
    });

    photoPick.addEventListener("click", function () {
        photoInput.click();
    });

    photoInput.addEventListener("change", function () {
        loadPhoto(photoInput.files[0]);
    });

    zoomInput.addEventListener("input", renderCrop);
    cropCanvas.addEventListener("mousedown", startCropDrag);
    cropCanvas.addEventListener("touchstart", startCropDrag, { passive: false });
    window.addEventListener("mousemove", moveCropDrag);
    window.addEventListener("touchmove", moveCropDrag, { passive: false });
    window.addEventListener("mouseup", stopCropDrag);
    window.addEventListener("touchend", stopCropDrag);
    photoSave.addEventListener("click", savePhoto);
    form.addEventListener("submit", saveProfile);
    passwordReset.addEventListener("click", sendPasswordReset);
    twoFactorConfirmButton.addEventListener("click", confirmTwoFactorSetting);
})();
