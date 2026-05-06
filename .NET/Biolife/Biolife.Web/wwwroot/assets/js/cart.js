;(function () {
    'use strict';

    const storageKey = 'biolife.cart.v1';
    const money = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' });
    const state = {
        items: [],
        isAuthenticated: false
    };

    const bySelector = (selector, root = document) => Array.from(root.querySelectorAll(selector));

    const escapeHtml = (value) => String(value || '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');

    const clampQuantity = (quantity) => {
        const parsed = parseInt(quantity, 10);
        if (Number.isNaN(parsed)) return 1;
        return Math.min(Math.max(parsed, 1), 99);
    };

    const normalizeUrl = (url) => {
        if (!url) return '/assets/images/products/p-01.jpg';
        try {
            const parsed = new URL(url, window.location.origin);
            return parsed.pathname + parsed.search;
        } catch {
            return url;
        }
    };

    const readLocal = () => {
        try {
            const parsed = JSON.parse(localStorage.getItem(storageKey) || '[]');
            return Array.isArray(parsed) ? parsed.map(normalizeItem).filter(Boolean) : [];
        } catch {
            return [];
        }
    };

    const writeLocal = (items) => {
        localStorage.setItem(storageKey, JSON.stringify(items.map(toInput)));
    };

    const clearLocal = () => localStorage.removeItem(storageKey);

    const normalizeItem = (item) => {
        if (!item) return null;
        const productName = String(item.productName || item.ProductName || '').trim();
        const imageUrl = normalizeUrl(item.imageUrl || item.ImageUrl);
        const unitPrice = Number(item.unitPrice ?? item.UnitPrice ?? 0);
        const quantity = clampQuantity(item.quantity ?? item.Quantity ?? 1);
        const productId = item.productId ?? item.ProductId ?? null;
        const productKey = String(item.productKey || item.ProductKey || productId || `${productName}|${imageUrl}|${unitPrice.toFixed(2)}`).trim();

        if (!productName) return null;

        return {
            id: item.id ?? item.Id ?? null,
            productId,
            productKey,
            productName,
            imageUrl,
            unitPrice: Math.max(0, Math.round(unitPrice * 100) / 100),
            quantity
        };
    };

    const toInput = (item) => ({
        id: item.id,
        productId: item.productId,
        productKey: item.productKey,
        productName: item.productName,
        imageUrl: item.imageUrl,
        unitPrice: item.unitPrice,
        quantity: item.quantity
    });

    const parsePrice = (element) => {
        const priceText = element?.querySelector('ins .price-amount, .price-amount')?.textContent || '0';
        const cleaned = priceText.replace(/[^0-9.,]/g, '').replace(',', '.');
        const parsed = parseFloat(cleaned);
        return Number.isNaN(parsed) ? 0 : parsed;
    };

    const productFromButton = (button) => {
        const product = button.closest('.contain-product, .biolife-quickview-inner, .sumary-product, .product-item');
        if (!product) return null;

        const name = product.querySelector('.pr-name, .product-name, .product-title a')?.textContent?.trim();
        const image = product.querySelector('.product-thumnail, .product-thumb img, .media img, img')?.getAttribute('src');
        const productId = button.dataset.productId || product.dataset.productId || null;
        const unitPrice = Number(button.dataset.price || product.dataset.price || parsePrice(product));
        const qtyInput = product.querySelector('.qty-input input');
        const quantity = clampQuantity(button.dataset.quantity || qtyInput?.value || 1);
        const imageUrl = normalizeUrl(image);

        if (!name) return null;

        return normalizeItem({
            productId: productId ? parseInt(productId, 10) : null,
            productKey: productId ? `product:${productId}` : `${name}|${imageUrl}|${unitPrice.toFixed(2)}`,
            productName: name,
            imageUrl,
            unitPrice,
            quantity
        });
    };

    const api = async (url, options = {}) => {
        const response = await fetch(url, {
            headers: { 'Content-Type': 'application/json', ...(options.headers || {}) },
            credentials: 'same-origin',
            ...options
        });

        if (response.status === 401) {
            state.isAuthenticated = false;
            throw new Error('Unauthorized');
        }

        if (!response.ok) {
            let message = `Cart request failed: ${response.status}`;
            try {
                const data = await response.clone().json();
                if (data && data.message) message = data.message;
            } catch {
            }
            throw new Error(message);
        }
        return response.json();
    };

    const setItems = (items, isAuthenticated = state.isAuthenticated) => {
        state.items = (items || []).map(normalizeItem).filter(Boolean);
        state.isAuthenticated = isAuthenticated;
        if (!state.isAuthenticated) writeLocal(state.items);
        render();
    };

    const setCheckoutStatus = (message, type = 'success') => {
        bySelector('.js-cart-checkout-status').forEach((node) => {
            node.textContent = message || '';
            node.className = `bio-cart-checkout-status js-cart-checkout-status${message ? ' is-visible' : ''}${type === 'error' ? ' is-error' : ' is-success'}`;
        });
    };

    const setButtonBusy = (button, busy, text) => {
        if (!button) return;
        if (busy) {
            button.dataset.originalText = button.textContent;
            button.textContent = text;
            button.disabled = true;
            return;
        }

        if (button.dataset.originalText) {
            button.textContent = button.dataset.originalText;
            delete button.dataset.originalText;
        }
        button.disabled = false;
    };

    const totals = () => {
        const count = state.items.reduce((sum, item) => sum + item.quantity, 0);
        const subtotal = state.items.reduce((sum, item) => sum + item.quantity * item.unitPrice, 0);
        return { count, subtotal };
    };

    const render = () => {
        const { count, subtotal } = totals();
        bySelector('.js-cart-count').forEach((node) => node.textContent = count);
        bySelector('.js-cart-summary').forEach((node) => node.textContent = `${count} item${count === 1 ? '' : 's'} - ${money.format(subtotal)}`);
        renderMiniCart();
        renderCartPage();
    };

    const renderMiniCart = () => {
        const lists = bySelector('.js-minicart-products');
        if (!lists.length) return;

        lists.forEach((list) => {
            if (!state.items.length) {
                list.innerHTML = '<li class="minicart-empty">No product in your cart</li>';
                return;
            }

            list.innerHTML = state.items.slice(0, 5).map((item) => `
                <li>
                    <div class="minicart-item" data-cart-id="${item.id || ''}" data-cart-key="${escapeHtml(item.productKey)}">
                        <div class="thumb">
                            <a href="/Cart"><img src="${escapeHtml(item.imageUrl)}" width="90" height="90" alt="${escapeHtml(item.productName)}"></a>
                        </div>
                        <div class="left-info">
                            <div class="product-title"><a href="/Cart" class="product-name">${escapeHtml(item.productName)}</a></div>
                            <div class="price"><ins><span class="price-amount"><span class="currencySymbol">$</span>${item.unitPrice.toFixed(2)}</span></ins></div>
                            <div class="qty">
                                <label>Qty:</label>
                                <input type="number" class="input-qty" value="${item.quantity}" min="1" max="99" disabled>
                            </div>
                        </div>
                        <div class="action">
                            <a href="#" class="js-cart-remove" aria-label="Remove item"><i class="fa fa-trash-o" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </li>
            `).join('');
        });
    };

    const renderCartPage = () => {
        const page = document.querySelector('.js-cart-page');
        if (!page) return;

        const { count, subtotal } = totals();
        const body = page.querySelector('.js-cart-page-items');
        const subtotalNodes = bySelector('.js-cart-subtotal', page);
        const totalNodes = bySelector('.js-cart-total', page);
        const countNodes = bySelector('.js-cart-page-count', page);

        countNodes.forEach((node) => node.textContent = `${count} item${count === 1 ? '' : 's'}`);
        subtotalNodes.forEach((node) => node.textContent = money.format(subtotal));
        totalNodes.forEach((node) => node.textContent = money.format(subtotal));

        if (!body) return;

        if (!state.items.length) {
            body.innerHTML = `
                <div class="bio-cart-empty">
                    <span class="biolife-icon icon-cart-mini"></span>
                    <h3>Your cart is empty</h3>
                    <p>Choose fresh products from the catalog and they will appear here.</p>
                    <a href="/" class="bio-cart-primary">Continue shopping</a>
                </div>
            `;
            return;
        }

        body.innerHTML = state.items.map((item) => `
            <article class="bio-cart-item" data-cart-id="${item.id || ''}" data-cart-key="${escapeHtml(item.productKey)}">
                <a href="/" class="bio-cart-thumb"><img src="${escapeHtml(item.imageUrl)}" alt="${escapeHtml(item.productName)}"></a>
                <div class="bio-cart-info">
                    <h3>${escapeHtml(item.productName)}</h3>
                    <span>${money.format(item.unitPrice)} each</span>
                </div>
                <div class="bio-cart-qty" aria-label="Quantity">
                    <button type="button" class="js-cart-decrease">-</button>
                    <input type="number" class="js-cart-quantity" min="1" max="99" value="${item.quantity}">
                    <button type="button" class="js-cart-increase">+</button>
                </div>
                <strong class="bio-cart-line-total">${money.format(item.unitPrice * item.quantity)}</strong>
                <button type="button" class="bio-cart-remove js-cart-remove" aria-label="Remove item">
                    <i class="fa fa-trash-o" aria-hidden="true"></i>
                </button>
            </article>
        `).join('');
    };

    const findItemElement = (target) => target.closest('[data-cart-id], [data-cart-key]');

    const findItem = (target) => {
        const element = findItemElement(target);
        if (!element) return null;
        const id = element.dataset.cartId ? parseInt(element.dataset.cartId, 10) : null;
        const key = element.dataset.cartKey;
        return state.items.find((item) => (id && item.id === id) || (!id && item.productKey === key)) || null;
    };

    const addItem = async (item) => {
        if (!item) return;

        if (state.isAuthenticated) {
            try {
                const result = await api('/Cart/Add', { method: 'POST', body: JSON.stringify(toInput(item)) });
                setItems(result.items, true);
                return;
            } catch {
                state.isAuthenticated = false;
            }
        }

        const existing = state.items.find((cartItem) => cartItem.productKey === item.productKey);
        if (existing) {
            existing.quantity = clampQuantity(existing.quantity + item.quantity);
        } else {
            state.items.unshift(item);
        }
        setItems(state.items, false);
    };

    const updateItem = async (item, quantity) => {
        quantity = clampQuantity(quantity);

        if (state.isAuthenticated && item.id) {
            const result = await api(`/Cart/Update/${item.id}`, { method: 'PUT', body: JSON.stringify({ quantity }) });
            setItems(result.items, true);
            return;
        }

        item.quantity = quantity;
        setItems(state.items, false);
    };

    const removeItem = async (item) => {
        if (state.isAuthenticated && item.id) {
            const result = await api(`/Cart/Remove/${item.id}`, { method: 'DELETE' });
            setItems(result.items, true);
            return;
        }

        setItems(state.items.filter((cartItem) => cartItem.productKey !== item.productKey), false);
    };

    const clearCart = async () => {
        if (state.isAuthenticated) {
            const result = await api('/Cart/Clear', { method: 'POST', body: '{}' });
            setItems(result.items, true);
            return;
        }

        setItems([], false);
    };

    const checkout = async (button) => {
        if (!state.items.length) {
            setCheckoutStatus('Your cart is empty.', 'error');
            return;
        }

        if (!state.isAuthenticated) {
            setCheckoutStatus('Please sign in and complete your profile before placing an order.', 'error');
            return;
        }

        setButtonBusy(button, true, 'Placing order...');
        setCheckoutStatus('', 'success');

        try {
            const result = await api('/Cart/Checkout', { method: 'POST', body: '{}' });
            setItems(result.items, true);
            setCheckoutStatus(result.message || 'Order placed successfully.', 'success');
        } catch (error) {
            setCheckoutStatus(error.message || 'Could not place the order.', 'error');
        } finally {
            setButtonBusy(button, false);
        }
    };

    const init = async () => {
        const localItems = readLocal();

        try {
            const result = await api('/Cart/Items');
            state.isAuthenticated = result.isAuthenticated === true;

            if (state.isAuthenticated && localItems.length) {
                const merged = await api('/Cart/MergeLocal', {
                    method: 'POST',
                    body: JSON.stringify({ items: localItems.map(toInput) })
                });
                clearLocal();
                setItems(merged.items, true);
                return;
            }

            setItems(state.isAuthenticated ? result.items : localItems, state.isAuthenticated);
        } catch {
            setItems(localItems, false);
        }
    };

    document.addEventListener('click', (event) => {
        const addButton = event.target.closest('.add-to-cart-btn');
        if (addButton) {
            event.preventDefault();
            addItem(productFromButton(addButton));
            return;
        }

        const removeButton = event.target.closest('.js-cart-remove');
        if (removeButton) {
            event.preventDefault();
            const item = findItem(removeButton);
            if (item) removeItem(item);
            return;
        }

        const decreaseButton = event.target.closest('.js-cart-decrease');
        if (decreaseButton) {
            const item = findItem(decreaseButton);
            if (item) updateItem(item, item.quantity - 1);
            return;
        }

        const increaseButton = event.target.closest('.js-cart-increase');
        if (increaseButton) {
            const item = findItem(increaseButton);
            if (item) updateItem(item, item.quantity + 1);
            return;
        }

        const clearButton = event.target.closest('.js-cart-clear');
        if (clearButton) {
            event.preventDefault();
            clearCart();
            return;
        }

        const checkoutButton = event.target.closest('.js-cart-checkout');
        if (checkoutButton) {
            event.preventDefault();
            checkout(checkoutButton);
        }
    });

    document.addEventListener('change', (event) => {
        const quantityInput = event.target.closest('.js-cart-quantity');
        if (!quantityInput) return;

        const item = findItem(quantityInput);
        if (item) updateItem(item, quantityInput.value);
    });

    document.addEventListener('DOMContentLoaded', init);
})();


