(() => {
    const body = document.body;
    if (!body) {
        return;
    }

    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    const addReadyClass = () => {
        requestAnimationFrame(() => {
            body.classList.add("auth-ready");
        });
    };

    const initParticles = () => {
        const bg = document.querySelector(".login-bg");
        if (!bg || reduceMotion) {
            return;
        }

        const layer = document.createElement("div");
        layer.className = "bg-particles";

        for (let i = 0; i < 16; i += 1) {
            const node = document.createElement("span");
            node.className = "bg-particle";
            node.style.setProperty("--x", `${Math.floor(Math.random() * 100)}`);
            node.style.setProperty("--y", `${Math.floor(Math.random() * 100)}`);
            node.style.setProperty("--size", `${2 + Math.floor(Math.random() * 4)}`);
            node.style.setProperty("--dur", `${8 + Math.random() * 6}s`);
            node.style.setProperty("--delay", `${Math.random() * -10}s`);
            node.style.setProperty("--alpha", `${0.2 + Math.random() * 0.45}`);
            layer.appendChild(node);
        }

        bg.appendChild(layer);
    };

    const initCardTilt = () => {
        const card = document.querySelector(".login-card");
        if (!card || reduceMotion) {
            return;
        }

        const maxTilt = 6;
        const onPointerMove = (event) => {
            const rect = card.getBoundingClientRect();
            const px = (event.clientX - rect.left) / rect.width;
            const py = (event.clientY - rect.top) / rect.height;
            const rx = (0.5 - py) * maxTilt * 2;
            const ry = (px - 0.5) * maxTilt * 2;

            card.classList.add("is-tilt");
            card.style.setProperty("--rx", `${rx.toFixed(2)}deg`);
            card.style.setProperty("--ry", `${ry.toFixed(2)}deg`);
        };

        const onLeave = () => {
            card.style.setProperty("--rx", "0deg");
            card.style.setProperty("--ry", "0deg");
            card.classList.remove("is-tilt");
        };

        card.addEventListener("pointermove", onPointerMove);
        card.addEventListener("pointerleave", onLeave);
    };

    const initSubmitLoading = () => {
        const form = document.querySelector(".login-form");
        if (!form) {
            return;
        }

        form.addEventListener("submit", (event) => {
            if (event.defaultPrevented || !form.checkValidity()) {
                return;
            }

            const submitButton = form.querySelector(".btn-login");
            if (submitButton) {
                submitButton.classList.add("loading");
            }
        });
    };

    const initNameLimit = () => {
        const input = document.querySelector("[data-name-max-length]");
        if (!input) {
            return;
        }

        const maxLength = Number(input.dataset.nameMaxLength) || 25;
        const wrapper = input.closest(".input-wrapper");
        const message = input.dataset.limitMessage
            ? document.querySelector(input.dataset.limitMessage)
            : null;
        const form = input.closest("form");

        const setExceeded = (isExceeded, shouldShake = false) => {
            wrapper?.classList.toggle("limit-exceeded", isExceeded);
            message?.classList.toggle("is-visible", isExceeded);
            input.setAttribute("aria-invalid", isExceeded ? "true" : "false");

            if (shouldShake && wrapper && !reduceMotion) {
                wrapper.classList.remove("is-shaking");
                void wrapper.offsetWidth;
                wrapper.classList.add("is-shaking");
            }
        };

        const syncState = () => {
            setExceeded(input.value.length > maxLength);
        };

        input.addEventListener("beforeinput", (event) => {
            if (event.isComposing || !event.inputType?.startsWith("insert")) {
                return;
            }

            const start = input.selectionStart ?? input.value.length;
            const end = input.selectionEnd ?? input.value.length;
            const insertedText = event.data ?? "";
            const nextLength = input.value.length - (end - start) + insertedText.length;

            if (nextLength <= maxLength) {
                return;
            }

            event.preventDefault();

            const availableLength = maxLength - (input.value.length - (end - start));
            if (insertedText && availableLength > 0) {
                const nextValue = input.value.slice(0, start)
                    + insertedText.slice(0, availableLength)
                    + input.value.slice(end);
                const cursorPosition = start + availableLength;

                input.value = nextValue;
                input.setSelectionRange(cursorPosition, cursorPosition);
                input.dispatchEvent(new Event("input", { bubbles: true }));
            }

            setExceeded(true, true);
        });

        input.addEventListener("input", syncState);

        form?.addEventListener("submit", (event) => {
            if (input.value.length <= maxLength) {
                return;
            }

            event.preventDefault();
            setExceeded(true, true);
            input.focus();
        });

        syncState();
    };

    addReadyClass();
    initParticles();
    initCardTilt();
    initNameLimit();
    initSubmitLoading();
})();
