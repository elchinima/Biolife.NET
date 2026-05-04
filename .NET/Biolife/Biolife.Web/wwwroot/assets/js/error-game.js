(function () {
    const root = document.querySelector("[data-error-game]");

    if (!root) {
        return;
    }

    const canvas = root.querySelector("[data-game-canvas]");
    const scoreValue = root.querySelector("[data-game-score]");
    const statusText = root.querySelector("[data-game-status]");
    const overlay = root.querySelector("[data-game-overlay]");
    const message = root.querySelector("[data-game-message]");
    const startButton = root.querySelector("[data-game-start]");
    const context = canvas.getContext("2d");
    const keys = new Set();
    const items = [];
    const state = {
        running: false,
        score: 0,
        misses: 0,
        lastTime: 0,
        spawnTimer: 0,
        basket: {
            x: 0,
            y: 0,
            width: 92,
            height: 32,
            speed: 430
        }
    };

    const produce = [
        { type: "good", color: "#e34235", leaf: "#69a84f", points: 10 },
        { type: "good", color: "#f2b84b", leaf: "#5fa85b", points: 12 },
        { type: "good", color: "#6fbf73", leaf: "#3f8d55", points: 14 },
        { type: "bad", color: "#d94b3d", points: -18 }
    ];

    function resize() {
        const rect = canvas.getBoundingClientRect();
        const ratio = window.devicePixelRatio || 1;

        canvas.width = Math.max(1, Math.round(rect.width * ratio));
        canvas.height = Math.max(1, Math.round(rect.height * ratio));
        context.setTransform(ratio, 0, 0, ratio, 0, 0);
        state.basket.y = rect.height - 48;

        if (!state.basket.x) {
            state.basket.x = rect.width / 2 - state.basket.width / 2;
        }
    }

    function reset() {
        const rect = canvas.getBoundingClientRect();

        state.running = true;
        state.score = 0;
        state.misses = 0;
        state.lastTime = 0;
        state.spawnTimer = 0;
        state.basket.x = rect.width / 2 - state.basket.width / 2;
        items.length = 0;
        overlay.hidden = true;
        updateStats();
        requestAnimationFrame(loop);
    }

    function updateStats() {
        scoreValue.textContent = String(state.score);
        statusText.textContent = `${3 - state.misses} chances left`;
    }

    function finish() {
        state.running = false;
        message.textContent = state.score >= 120
            ? `Nice save. You collected ${state.score} fresh points and found the path back.`
            : `You collected ${state.score} fresh points. Try one more round while the page recovers.`;
        startButton.textContent = "Play again";
        overlay.hidden = false;
    }

    function spawnItem() {
        const rect = canvas.getBoundingClientRect();
        const model = produce[Math.floor(Math.random() * produce.length)];
        const size = model.type === "bad" ? 28 : 30 + Math.random() * 10;

        items.push({
            type: model.type,
            color: model.color,
            leaf: model.leaf,
            points: model.points,
            x: 20 + Math.random() * Math.max(1, rect.width - 40),
            y: -30,
            radius: size / 2,
            speed: 105 + Math.random() * 95,
            rotation: Math.random() * Math.PI
        });
    }

    function moveBasket(delta) {
        const rect = canvas.getBoundingClientRect();
        const basket = state.basket;
        let direction = 0;

        if (keys.has("ArrowLeft") || keys.has("a")) {
            direction -= 1;
        }

        if (keys.has("ArrowRight") || keys.has("d")) {
            direction += 1;
        }

        basket.x += direction * basket.speed * delta;
        basket.x = Math.max(8, Math.min(rect.width - basket.width - 8, basket.x));
    }

    function updateItems(delta) {
        const rect = canvas.getBoundingClientRect();
        const basket = state.basket;

        for (let index = items.length - 1; index >= 0; index -= 1) {
            const item = items[index];
            item.y += item.speed * delta;
            item.rotation += delta * 2;

            const caught =
                item.y + item.radius >= basket.y &&
                item.y - item.radius <= basket.y + basket.height &&
                item.x >= basket.x &&
                item.x <= basket.x + basket.width;

            if (caught) {
                state.score = Math.max(0, state.score + item.points);
                items.splice(index, 1);
                updateStats();
                continue;
            }

            if (item.y - item.radius > rect.height) {
                if (item.type === "good") {
                    state.misses += 1;
                    updateStats();
                }

                items.splice(index, 1);
            }
        }

        if (state.misses >= 3 || state.score >= 180) {
            finish();
        }
    }

    function drawBackground(width, height) {
        context.clearRect(0, 0, width, height);
        context.fillStyle = "rgba(255, 255, 255, 0.34)";

        for (let x = 26; x < width; x += 52) {
            context.beginPath();
            context.moveTo(x, 0);
            context.lineTo(x + 24, height);
            context.strokeStyle = "rgba(5, 165, 3, 0.08)";
            context.stroke();
        }
    }

    function drawProduce(item) {
        context.save();
        context.translate(item.x, item.y);
        context.rotate(item.rotation);

        if (item.type === "bad") {
            context.fillStyle = item.color;
            context.strokeStyle = "#fff";
            context.lineWidth = 3;
            context.beginPath();
            context.rect(-item.radius, -item.radius, item.radius * 2, item.radius * 2);
            context.fill();
            context.stroke();
            context.strokeStyle = "#fff";
            context.lineWidth = 2;
            context.beginPath();
            context.moveTo(-7, -7);
            context.lineTo(7, 7);
            context.moveTo(7, -7);
            context.lineTo(-7, 7);
            context.stroke();
        } else {
            context.fillStyle = item.color;
            context.beginPath();
            context.arc(0, 2, item.radius, 0, Math.PI * 2);
            context.fill();
            context.fillStyle = item.leaf;
            context.beginPath();
            context.ellipse(7, -item.radius + 4, 9, 5, -0.55, 0, Math.PI * 2);
            context.fill();
            context.fillStyle = "rgba(255, 255, 255, 0.36)";
            context.beginPath();
            context.arc(-5, -3, item.radius * 0.28, 0, Math.PI * 2);
            context.fill();
        }

        context.restore();
    }

    function drawBasket() {
        const basket = state.basket;

        context.fillStyle = "#8a5a2b";
        context.fillRect(basket.x + 9, basket.y + 10, basket.width - 18, basket.height - 6);
        context.fillStyle = "#b9853a";
        context.fillRect(basket.x, basket.y, basket.width, 15);
        context.strokeStyle = "#6d4621";
        context.lineWidth = 3;
        context.beginPath();
        context.arc(basket.x + basket.width / 2, basket.y + 9, basket.width * 0.36, Math.PI, Math.PI * 2);
        context.stroke();

        for (let x = basket.x + 15; x < basket.x + basket.width - 8; x += 18) {
            context.strokeStyle = "rgba(255, 255, 255, 0.28)";
            context.beginPath();
            context.moveTo(x, basket.y + 2);
            context.lineTo(x - 8, basket.y + basket.height - 2);
            context.stroke();
        }
    }

    function render() {
        const rect = canvas.getBoundingClientRect();

        drawBackground(rect.width, rect.height);
        items.forEach(drawProduce);
        drawBasket();
    }

    function loop(time) {
        if (!state.running) {
            render();
            return;
        }

        const delta = state.lastTime ? Math.min((time - state.lastTime) / 1000, 0.034) : 0;
        state.lastTime = time;
        state.spawnTimer -= delta;

        if (state.spawnTimer <= 0) {
            spawnItem();
            state.spawnTimer = Math.max(0.38, 0.9 - state.score / 260);
        }

        moveBasket(delta);
        updateItems(delta);
        render();
        requestAnimationFrame(loop);
    }

    function moveToClientX(clientX) {
        const rect = canvas.getBoundingClientRect();
        state.basket.x = clientX - rect.left - state.basket.width / 2;
        state.basket.x = Math.max(8, Math.min(rect.width - state.basket.width - 8, state.basket.x));
    }

    window.addEventListener("resize", () => {
        resize();
        render();
    });

    window.addEventListener("keydown", (event) => {
        if (["ArrowLeft", "ArrowRight", "a", "d"].includes(event.key)) {
            keys.add(event.key);
            event.preventDefault();
        }
    });

    window.addEventListener("keyup", (event) => {
        keys.delete(event.key);
    });

    canvas.addEventListener("pointermove", (event) => {
        moveToClientX(event.clientX);
    });

    canvas.addEventListener("pointerdown", (event) => {
        canvas.setPointerCapture(event.pointerId);
        moveToClientX(event.clientX);
    });

    startButton.addEventListener("click", reset);

    resize();
    render();
})();
