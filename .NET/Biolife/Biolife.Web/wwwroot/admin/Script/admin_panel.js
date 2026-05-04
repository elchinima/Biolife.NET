const adminLanguageStorageKey = 'adminPanelLanguage';
const adminLanguages = ['az', 'en', 'ru'];
const adminLanguageLabels = { az: 'Az', en: 'En', ru: 'Ru' };
const adminLanguageLocales = { az: 'az-AZ', en: 'en-US', ru: 'ru-RU' };
const adminTranslations = {
    az: {
        'Language': 'Dil',
        'Dashboard': 'Panel',
        'Products': 'Məhsullar',
        'Books / Products': 'Kitablar / Məhsullar',
        'Slider': 'Slayder',
        'Authors': 'Müəlliflər',
        'Genres': 'Janrlar',
        'Users': 'İstifadəçilər',
        'Roles': 'Rollar',
        'Notes': 'Qeydlər',
        'Logout': 'Çıxış',
        'Exit to site': 'Sayta keç',
        'Admin Panel': 'Admin Panel',
        'Total Orders': 'Ümumi sifarişlər',
        'Revenue': 'Gəlir',
        'Books in Catalog': 'Kataloqdakı kitablar',
        'Sales in the Last 7 Days': 'Son 7 gündə satış',
        '+12% this month': 'Bu ay +12%',
        '+8% this month': 'Bu ay +8%',
        'No changes': 'Dəyişiklik yoxdur',
        '+34 today': 'Bu gün +34',
        'ID': 'ID',
        'Cover': 'Üzlük',
        'Name': 'Ad',
        'Email': 'E-poçt',
        'Role': 'Rol',
        'Registered': 'Qeydiyyat',
        'Last Login': 'Son giriş',
        'Status': 'Status',
        'Author': 'Müəllif',
        'Genre': 'Janr',
        'Price': 'Qiymət',
        'Featured': 'Seçilmiş',
        'Actions': 'Əməliyyatlar',
        'Order': 'Sıra',
        'Color': 'Rəng',
        'Permissions': 'İcazələr',
        'Books': 'Kitablar',
        'Active': 'Aktiv',
        'Blocked': 'Bloklanıb',
        'Yes': 'Bəli',
        'No': 'Xeyr',
        'Never': 'Heç vaxt',
        'No role': 'Rol yoxdur',
        'Admin Panel': 'Admin Panel',
        'Create Notes': 'Qeyd yaratma',
        'Search product...': 'Məhsul axtar...',
        'Search genre...': 'Janr axtar...',
        'Search author...': 'Müəllif axtar...',
        'Search users...': 'İstifadəçi axtar...',
        'Search role...': 'Rol axtar...',
        'Search note...': 'Qeyd axtar...',
        'Search user by name or email...': 'Ad və ya e-poçt ilə axtar...',
        'All Status': 'Bütün statuslar',
        '+ Add Product': '+ Məhsul əlavə et',
        '+ Add Genre': '+ Janr əlavə et',
        '+ Add Author': '+ Müəllif əlavə et',
        '+ Add Role': '+ Rol əlavə et',
        '+ Add Note': '+ Qeyd əlavə et',
        '+ Add Slide': '+ Slayd əlavə et',
        'Add Product': 'Məhsul əlavə et',
        'Edit Product': 'Məhsulu redaktə et',
        'Add Genre': 'Janr əlavə et',
        'Edit Genre': 'Janrı redaktə et',
        'Add Author': 'Müəllif əlavə et',
        'Edit Author': 'Müəllifi redaktə et',
        'Add Role': 'Rol əlavə et',
        'Edit Role': 'Rolu redaktə et',
        'Add Note': 'Qeyd əlavə et',
        'Create Note': 'Qeyd yarat',
        'Add Slide': 'Slayd əlavə et',
        'Change User Name': 'İstifadəçi adını dəyiş',
        'Manage Role Users': 'Rol istifadəçiləri',
        'Manage Users': 'İstifadəçiləri idarə et',
        'Name *': 'Ad *',
        'Role Name *': 'Rol adı *',
        'Price (₼) *': 'Qiymət (₼) *',
        'Cost Price (₼)': 'Maya dəyəri (₼)',
        'Discount %': 'Endirim %',
        'Description': 'Təsvir',
        'Title': 'Başlıq',
        'Title *': 'Başlıq *',
        'Subtitle': 'Alt başlıq',
        'Text *': 'Mətn *',
        'Type *': 'Növ *',
        'Button Text': 'Düymə mətni',
        'Button Link': 'Düymə linki',
        'Background Image': 'Fon şəkli',
        'Cover Image': 'Üzlük şəkli',
        'Cover Image *': 'Üzlük şəkli *',
        'Cover Image (optional)': 'Üzlük şəkli (istəyə bağlı)',
        'Hover Image': 'Hover şəkli',
        'Hover Image (optional)': 'Hover şəkli (istəyə bağlı)',
        'Upload': 'Yüklə',
        'Upload Image': 'Şəkil yüklə',
        'Upload cover image': 'Üzlük şəklini yüklə',
        'Upload hover image': 'Hover şəklini yüklə',
        'Active': 'Aktiv',
        'New': 'Yeni',
        'New Arrival': 'Yeni gələn',
        'Normal': 'Normal',
        'High': 'Yüksək',
        'Immediate': 'Təcili',
        'Save': 'Yadda saxla',
        'Save Roles': 'Rolları saxla',
        'Cancel': 'Ləğv et',
        'Close': 'Bağla',
        'Delete': 'Sil',
        'Delete Slide': 'Slaydı sil',
        'Confirm Deletion': 'Silməni təsdiqlə',
        'Are you sure you want to delete this product?': 'Bu məhsulu silmək istədiyinizə əminsiniz?',
        'Are you sure you want to delete this genre?': 'Bu janrı silmək istədiyinizə əminsiniz?',
        'Are you sure you want to delete this author?': 'Bu müəllifi silmək istədiyinizə əminsiniz?',
        'Are you sure you want to delete this slide?': 'Bu slaydı silmək istədiyinizə əminsiniz?',
        'Access denied': 'Giriş qadağandır',
        'You do not have permission to open this section.': 'Bu bölməni açmaq üçün icazəniz yoxdur.',
        'Access to this section is restricted by your role settings.': 'Bu bölməyə giriş rol ayarlarınızla məhdudlaşdırılıb.',
        'Back to dashboard': 'Panelə qayıt',
        'Go to site': 'Sayta keç',
        'Details': 'Detallar',
        'Open row details': 'Sətir detallarını aç',
        'Pin navbar': 'Menyunu sabitlə',
        'Unpin navbar': 'Menyunu sabitdən çıxar',
        'Clear selected file': 'Seçilmiş faylı təmizlə',
        'Only JPG, PNG and GIF images are allowed.': 'Yalnız JPG, PNG və GIF şəkillərinə icazə verilir.',
        'Actions': 'Əməliyyatlar',
        'Change Name': 'Adı dəyiş',
        'Block': 'Blokla',
        'Unblock': 'Blokdan çıxar',
        'Blocked by hierarchy': 'İyerarxiya ilə bloklanıb',
        'Protected by hierarchy': 'İyerarxiya ilə qorunur',
        'No notes yet.': 'Hələ qeyd yoxdur.',
        'Create Notes permission is required to add notes.': 'Qeyd əlavə etmək üçün Create Notes icazəsi lazımdır.',
        'Continue': 'Davam et',
        'Note': 'Qeyd',
        'Unknown': 'Naməlum',
        'Move Up': 'Yuxarı daşı',
        'Move Down': 'Aşağı daşı'
    },
    en: {},
    ru: {
        'Language': 'Язык',
        'Dashboard': 'Панель',
        'Products': 'Товары',
        'Books / Products': 'Книги / Товары',
        'Slider': 'Слайдер',
        'Authors': 'Авторы',
        'Genres': 'Жанры',
        'Users': 'Пользователи',
        'Roles': 'Роли',
        'Notes': 'Заметки',
        'Logout': 'Выход',
        'Exit to site': 'На сайт',
        'Admin Panel': 'Админ панель',
        'Total Orders': 'Всего заказов',
        'Revenue': 'Доход',
        'Books in Catalog': 'Книг в каталоге',
        'Sales in the Last 7 Days': 'Продажи за последние 7 дней',
        '+12% this month': '+12% за месяц',
        '+8% this month': '+8% за месяц',
        'No changes': 'Без изменений',
        '+34 today': '+34 сегодня',
        'ID': 'ID',
        'Cover': 'Обложка',
        'Name': 'Имя',
        'Email': 'Email',
        'Role': 'Роль',
        'Registered': 'Регистрация',
        'Last Login': 'Последний вход',
        'Status': 'Статус',
        'Author': 'Автор',
        'Genre': 'Жанр',
        'Price': 'Цена',
        'Featured': 'Избранное',
        'Actions': 'Действия',
        'Order': 'Порядок',
        'Color': 'Цвет',
        'Permissions': 'Права',
        'Books': 'Книги',
        'Active': 'Активен',
        'Blocked': 'Заблокирован',
        'Yes': 'Да',
        'No': 'Нет',
        'Never': 'Никогда',
        'No role': 'Нет роли',
        'Create Notes': 'Создание заметок',
        'Search product...': 'Поиск товара...',
        'Search genre...': 'Поиск жанра...',
        'Search author...': 'Поиск автора...',
        'Search users...': 'Поиск пользователей...',
        'Search role...': 'Поиск роли...',
        'Search note...': 'Поиск заметки...',
        'Search user by name or email...': 'Поиск по имени или email...',
        'All Status': 'Все статусы',
        '+ Add Product': '+ Добавить товар',
        '+ Add Genre': '+ Добавить жанр',
        '+ Add Author': '+ Добавить автора',
        '+ Add Role': '+ Добавить роль',
        '+ Add Note': '+ Добавить заметку',
        '+ Add Slide': '+ Добавить слайд',
        'Add Product': 'Добавить товар',
        'Edit Product': 'Редактировать товар',
        'Add Genre': 'Добавить жанр',
        'Edit Genre': 'Редактировать жанр',
        'Add Author': 'Добавить автора',
        'Edit Author': 'Редактировать автора',
        'Add Role': 'Добавить роль',
        'Edit Role': 'Редактировать роль',
        'Add Note': 'Добавить заметку',
        'Create Note': 'Создать заметку',
        'Add Slide': 'Добавить слайд',
        'Change User Name': 'Изменить имя пользователя',
        'Manage Role Users': 'Пользователи роли',
        'Manage Users': 'Управлять пользователями',
        'Name *': 'Имя *',
        'Role Name *': 'Название роли *',
        'Price (₼) *': 'Цена (₼) *',
        'Cost Price (₼)': 'Себестоимость (₼)',
        'Discount %': 'Скидка %',
        'Description': 'Описание',
        'Title': 'Заголовок',
        'Title *': 'Заголовок *',
        'Subtitle': 'Подзаголовок',
        'Text *': 'Текст *',
        'Type *': 'Тип *',
        'Button Text': 'Текст кнопки',
        'Button Link': 'Ссылка кнопки',
        'Background Image': 'Фоновое изображение',
        'Cover Image': 'Изображение обложки',
        'Cover Image *': 'Изображение обложки *',
        'Cover Image (optional)': 'Изображение обложки (необязательно)',
        'Hover Image': 'Изображение при наведении',
        'Hover Image (optional)': 'Изображение при наведении (необязательно)',
        'Upload': 'Загрузить',
        'Upload Image': 'Загрузить изображение',
        'Upload cover image': 'Загрузить обложку',
        'Upload hover image': 'Загрузить hover-изображение',
        'New': 'Новый',
        'New Arrival': 'Новинка',
        'Normal': 'Обычная',
        'High': 'Высокая',
        'Immediate': 'Срочная',
        'Save': 'Сохранить',
        'Save Roles': 'Сохранить роли',
        'Cancel': 'Отмена',
        'Close': 'Закрыть',
        'Delete': 'Удалить',
        'Delete Slide': 'Удалить слайд',
        'Confirm Deletion': 'Подтверждение удаления',
        'Are you sure you want to delete this product?': 'Вы уверены, что хотите удалить этот товар?',
        'Are you sure you want to delete this genre?': 'Вы уверены, что хотите удалить этот жанр?',
        'Are you sure you want to delete this author?': 'Вы уверены, что хотите удалить этого автора?',
        'Are you sure you want to delete this slide?': 'Вы уверены, что хотите удалить этот слайд?',
        'Access denied': 'Доступ запрещён',
        'You do not have permission to open this section.': 'У вас нет прав для открытия этого раздела.',
        'Access to this section is restricted by your role settings.': 'Доступ к этому разделу ограничен настройками вашей роли.',
        'Back to dashboard': 'Вернуться в панель',
        'Go to site': 'Перейти на сайт',
        'Details': 'Детали',
        'Open row details': 'Открыть детали строки',
        'Pin navbar': 'Закрепить меню',
        'Unpin navbar': 'Открепить меню',
        'Clear selected file': 'Очистить выбранный файл',
        'Only JPG, PNG and GIF images are allowed.': 'Разрешены только изображения JPG, PNG и GIF.',
        'Change Name': 'Изменить имя',
        'Block': 'Заблокировать',
        'Unblock': 'Разблокировать',
        'Blocked by hierarchy': 'Заблокировано иерархией',
        'Protected by hierarchy': 'Защищено иерархией',
        'No notes yet.': 'Заметок пока нет.',
        'Create Notes permission is required to add notes.': 'Для добавления заметок нужно право Create Notes.',
        'Continue': 'Продолжить',
        'Note': 'Заметка',
        'Unknown': 'Неизвестно',
        'Move Up': 'Вверх',
        'Move Down': 'Вниз'
    }
};

adminTranslations.en = Object.keys({ ...adminTranslations.az, ...adminTranslations.ru })
    .reduce((items, key) => ({ ...items, [key]: key }), {});

const adminTextSources = new WeakMap();
const adminAttributeSources = new WeakMap();
let isApplyingAdminLanguage = false;

function getAdminLanguage() {
    const saved = localStorage.getItem(adminLanguageStorageKey);
    return adminLanguages.includes(saved) ? saved : 'en';
}

function getAdminTranslation(source, language = getAdminLanguage()) {
    return adminTranslations[language]?.[source] || source;
}

function normalizeAdminText(value) {
    return (value || '').replace(/\s+/g, ' ').trim();
}

function translateAdminTextNode(node, language) {
    const current = normalizeAdminText(node.nodeValue);
    if (!current) return;

    if (!adminTextSources.has(node) && adminTranslations.en[current]) {
        adminTextSources.set(node, current);
    }

    const source = adminTextSources.get(node);
    if (!source) return;

    const translated = getAdminTranslation(source, language);
    if (current !== translated) {
        node.nodeValue = (node.nodeValue || '').replace(current, translated);
    }
}

function translateAdminAttribute(element, attribute, language) {
    if (!element.hasAttribute(attribute)) return;

    let sources = adminAttributeSources.get(element);
    if (!sources) {
        sources = {};
        adminAttributeSources.set(element, sources);
    }

    const current = normalizeAdminText(element.getAttribute(attribute));
    if (!sources[attribute] && adminTranslations.en[current]) {
        sources[attribute] = current;
    }

    if (sources[attribute]) {
        element.setAttribute(attribute, getAdminTranslation(sources[attribute], language));
    }
}

function translateAdminDataElements(root, language) {
    if (root.nodeType === Node.ELEMENT_NODE && root.dataset?.i18n) {
        root.textContent = getAdminTranslation(root.dataset.i18n, language);
    }

    root.querySelectorAll?.('[data-i18n]').forEach(element => {
        const source = element.dataset.i18n;
        if (source) element.textContent = getAdminTranslation(source, language);
    });

    root.querySelectorAll?.('[data-i18n-placeholder]').forEach(element => {
        const source = element.dataset.i18nPlaceholder;
        if (source) element.setAttribute('placeholder', getAdminTranslation(source, language));
    });

    root.querySelectorAll?.('[data-i18n-title]').forEach(element => {
        const source = element.dataset.i18nTitle;
        if (source) element.setAttribute('title', getAdminTranslation(source, language));
    });
}

function translateAdminTextNodes(root, language) {
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, {
        acceptNode(node) {
            const parent = node.parentElement;
            if (!parent) return NodeFilter.FILTER_REJECT;
            if (parent.closest('script, style, textarea, input, .user-avatar')) {
                return NodeFilter.FILTER_REJECT;
            }
            return NodeFilter.FILTER_ACCEPT;
        }
    });

    const nodes = [];
    while (walker.nextNode()) nodes.push(walker.currentNode);
    nodes.forEach(node => translateAdminTextNode(node, language));
}

function translateAdminAttributes(root, language) {
    root.querySelectorAll?.('[placeholder], [title], [aria-label]').forEach(element => {
        translateAdminAttribute(element, 'placeholder', language);
        translateAdminAttribute(element, 'title', language);
        translateAdminAttribute(element, 'aria-label', language);
    });
}

function updateAdminLanguageSwitcher(language) {
    document.querySelectorAll('.admin-language-option').forEach(button => {
        const isActive = button.dataset.adminLang === language;
        button.classList.toggle('active', isActive);
        button.setAttribute('aria-pressed', isActive ? 'true' : 'false');
    });
}

function setTopbarDate(language = getAdminLanguage()) {
    const dateElement = document.getElementById('topbarDate');
    if (!dateElement) return;
    dateElement.textContent = new Date().toLocaleDateString(adminLanguageLocales[language], {
        day: 'numeric',
        month: 'long',
        year: 'numeric'
    });
}

function applyAdminLanguage(language = getAdminLanguage(), root = document.body) {
    if (!root || isApplyingAdminLanguage) return;

    isApplyingAdminLanguage = true;
    document.documentElement.lang = language;
    translateAdminDataElements(root, language);
    translateAdminTextNodes(root, language);
    translateAdminAttributes(root, language);
    updateAdminLanguageSwitcher(language);
    setTopbarDate(language);

    if (pageTitle?.id === 'pageTitle') {
        const savedPage = localStorage.getItem('adminActivePage');
        const activePage = savedPage && titles[savedPage] ? savedPage : 'dashboard';
        pageTitle.textContent = getAdminTranslation(titles[activePage] || pageTitle.textContent, language);
    }

    isApplyingAdminLanguage = false;
}

function setupAdminLanguageSwitcher() {
    const footer = document.querySelector('.sidebar-footer');
    if (!footer || footer.querySelector('.admin-language-switcher')) return;

    const switcher = document.createElement('div');
    switcher.className = 'admin-language-switcher';
    switcher.innerHTML = `
        <div class="admin-language-label" data-i18n="Language">Language</div>
        <div class="admin-language-options" role="group" aria-label="Language">
            ${adminLanguages.map(language => `
                <button type="button" class="admin-language-option" data-admin-lang="${language}" aria-pressed="false">
                    ${adminLanguageLabels[language]}
                </button>`).join('')}
        </div>`;

    footer.prepend(switcher);

    switcher.querySelectorAll('.admin-language-option').forEach(button => {
        button.addEventListener('click', () => {
            const language = button.dataset.adminLang;
            if (!adminLanguages.includes(language)) return;
            localStorage.setItem(adminLanguageStorageKey, language);
            applyAdminLanguage(language);
        });
    });
}

function observeAdminLanguageChanges() {
    const observer = new MutationObserver(mutations => {
        if (isApplyingAdminLanguage) return;
        const language = getAdminLanguage();

        mutations.forEach(mutation => {
            mutation.addedNodes.forEach(node => {
                if (node.nodeType === Node.TEXT_NODE) {
                    translateAdminTextNode(node, language);
                    return;
                }

                if (node.nodeType === Node.ELEMENT_NODE) {
                    applyAdminLanguage(language, node);
                }
            });
        });
    });

    observer.observe(document.body, { childList: true, subtree: true });
}

setupAdminLanguageSwitcher();

const navItems = document.querySelectorAll('.nav-item[data-page]');
const pages = document.querySelectorAll('.page');
const pageTitle = document.getElementById('pageTitle');
const titles = { dashboard: 'Dashboard', books: 'Books / Products', orders: 'Orders', slider: 'Slider' };
const prefersReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;
const allowedImageExtensions = ['jpg', 'jpeg', 'png', 'gif'];
const staticImageMaxBytes = 2 * 1024 * 1024;
const gifImageMaxBytes = 10 * 1024 * 1024;

function formatFileSize(bytes) {
    return `${Math.round(bytes / 1024 / 1024)} MB`;
}

function validateSelectedImageFile(file) {
    if (!file) return '';

    const extension = (file.name.split('.').pop() || '').toLowerCase();
    if (!allowedImageExtensions.includes(extension)) {
        return 'Only JPG, PNG and GIF images are allowed.';
    }

    const maxBytes = extension === 'gif' ? gifImageMaxBytes : staticImageMaxBytes;
    if (file.size > maxBytes) {
        return `Maximum file size for ${extension.toUpperCase()} is ${formatFileSize(maxBytes)}.`;
    }

    return '';
}

function animateInlinePage(page) {
    if (!page || prefersReducedMotion) return;
    page.classList.remove('page-switching');
    void page.getBoundingClientRect();
    page.classList.add('page-switching');
}

function activateInlinePage(target, persist = false) {
    if (!target) return;
    const page = document.getElementById('page-' + target);
    if (!page) return;

    navItems.forEach(n => n.classList.remove('active'));
    pages.forEach(p => p.classList.remove('active'));
    document.querySelector(`[data-page="${target}"]`)?.classList.add('active');
    page.classList.add('active');
    animateInlinePage(page);
    revealAnimatedBlocks(page);

    if (pageTitle && titles[target]) pageTitle.textContent = titles[target];
    if (persist) localStorage.setItem('adminActivePage', target);
}

if (navItems.length && pages.length) {
    navItems.forEach(item => {
        item.addEventListener('click', e => {
            e.preventDefault();
            activateInlinePage(item.dataset.page, true);
        });
    });
}

const savedPage = localStorage.getItem('adminActivePage');
if (savedPage && navItems.length && pages.length && document.getElementById('page-' + savedPage)) {
    activateInlinePage(savedPage, false);
}

document.querySelectorAll('[data-goto]').forEach(el => {
    el.addEventListener('click', e => {
        e.preventDefault();
        activateInlinePage(el.dataset.goto, true);
    });
});

const isMobile = () => window.innerWidth <= 768;
const sidebarOverlay = document.createElement('div');
sidebarOverlay.className = 'sidebar-overlay';
document.body.appendChild(sidebarOverlay);
const sidebar = document.getElementById('sidebar');
const mainWrapper = document.querySelector('.main-wrapper');
const sidebarPinnedStorageKey = 'adminSidebarPinned';
const sidebarCollapsedStorageKey = 'adminSidebarCollapsed';

function ensureSidebarPinButton() {
    const logo = document.querySelector('.sidebar-logo');
    if (!logo || logo.querySelector('.sidebar-pin-toggle')) return null;

    const button = document.createElement('button');
    button.type = 'button';
    button.className = 'sidebar-pin-toggle';
    button.setAttribute('aria-label', 'Pin navbar');
    button.setAttribute('title', 'Pin navbar');
    logo.appendChild(button);
    return button;
}

function setSidebarPinIcon(button, isPinned) {
    if (!button) return;
    const label = getAdminTranslation(isPinned ? 'Unpin navbar' : 'Pin navbar');
    button.classList.toggle('is-active', isPinned);
    button.setAttribute('aria-label', label);
    button.setAttribute('title', label);
    button.innerHTML = isPinned
        ? '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="4" y1="4" x2="20" y2="20"></line><path d="M12 17v5"></path><path d="M9 9V4h6v5l3 3v2H6v-2l3-3z"></path></svg>'
        : '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 17v5"></path><path d="M9 9V4h6v5l3 3v2H6v-2l3-3z"></path></svg>';
}

function applySidebarDesktopState() {
    if (!sidebar || !mainWrapper) return;
    if (isMobile()) {
        sidebar.classList.remove('is-pinned', 'collapsed');
        mainWrapper.classList.remove('expanded');
        return;
    }

    const isPinned = localStorage.getItem(sidebarPinnedStorageKey) === 'true';
    const isCollapsed = localStorage.getItem(sidebarCollapsedStorageKey) === 'true';
    sidebar.classList.toggle('is-pinned', isPinned);
    sidebar.classList.toggle('collapsed', !isPinned && isCollapsed);
    mainWrapper.classList.toggle('expanded', !isPinned && isCollapsed);
    setSidebarPinIcon(document.querySelector('.sidebar-pin-toggle'), isPinned);
}

const sidebarPinButton = ensureSidebarPinButton();
setSidebarPinIcon(sidebarPinButton, localStorage.getItem(sidebarPinnedStorageKey) === 'true');
sidebarPinButton?.addEventListener('click', event => {
    event.preventDefault();
    event.stopPropagation();
    if (isMobile()) return;

    const nextPinned = localStorage.getItem(sidebarPinnedStorageKey) !== 'true';
    localStorage.setItem(sidebarPinnedStorageKey, nextPinned ? 'true' : 'false');
    if (nextPinned) localStorage.setItem(sidebarCollapsedStorageKey, 'false');
    applySidebarDesktopState();
});

applySidebarDesktopState();
window.addEventListener('resize', applySidebarDesktopState);

document.getElementById('sidebarToggle')?.addEventListener('click', () => {
    if (isMobile()) {
        sidebar?.classList.toggle('mobile-open');
        sidebarOverlay.classList.toggle('active');
    } else {
        if (localStorage.getItem(sidebarPinnedStorageKey) === 'true') return;
        const nextCollapsed = !sidebar?.classList.contains('collapsed');
        localStorage.setItem(sidebarCollapsedStorageKey, nextCollapsed ? 'true' : 'false');
        applySidebarDesktopState();
    }
});

sidebarOverlay.addEventListener('click', () => {
    sidebar?.classList.remove('mobile-open');
    sidebarOverlay.classList.remove('active');
});

setTopbarDate();

let _pendingDeleteRow = null;

document.querySelectorAll('.btn-del').forEach(btn => {
    btn.addEventListener('click', () => {
        _pendingDeleteRow = btn.closest('form');
        openModal('confirmModal');
    });
});

document.querySelectorAll('.btn-slide-del').forEach(btn => {
    btn.addEventListener('click', () => {
        document.getElementById('deleteSlideId').value = btn.dataset.id;
        document.getElementById('confirmDeleteBtn').dataset.target = 'slide';
        openModal('confirmModal');
    });
});

document.getElementById('confirmDeleteBtn')?.addEventListener('click', () => {
    if (_pendingDeleteRow) {
        _pendingDeleteRow.submit();
        _pendingDeleteRow = null;
    } else if (document.getElementById('confirmDeleteBtn').dataset.target === 'slide') {
        document.getElementById('deleteSlideForm').submit();
    }
    closeModal('confirmModal');
});

document.getElementById('bookSearch')?.addEventListener('input', function () {
    const q = this.value.toLowerCase();
    document.querySelectorAll('#booksBody tr').forEach(row => {
        row.style.display = row.textContent.toLowerCase().includes(q) ? '' : 'none';
    });
});

document.querySelectorAll('.status-select').forEach(sel => {
    sel.addEventListener('change', function () {
        this.className = 'status-select';
        const map = { 'Delivered': 'status-success', 'In Transit': 'status-warning', 'Processing': 'status-info', 'Cancelled': 'status-danger' };
        this.classList.add(map[this.value] || '');
    });
});

function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    modal.classList.add('open');
    revealAnimatedBlocks(modal);
}

function closeModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;
    modal.classList.remove('open');
}

function getFileUploadFloatingPreview() {
    let preview = document.querySelector('.file-upload-floating-preview');
    if (preview) return preview;

    preview = document.createElement('div');
    preview.className = 'file-upload-floating-preview';
    preview.innerHTML = '<img alt="Image preview">';
    document.body.appendChild(preview);
    return preview;
}

function positionFileUploadFloatingPreview(preview, event, anchor) {
    const margin = 14;
    const rect = anchor.getBoundingClientRect();
    const previewWidth = preview.offsetWidth || 360;
    const previewHeight = preview.offsetHeight || 240;
    const pointerX = event?.clientX ?? rect.left;
    const pointerY = event?.clientY ?? rect.bottom;

    let left = pointerX + margin;
    let top = pointerY + margin;

    if (left + previewWidth > window.innerWidth - margin) {
        left = Math.max(margin, pointerX - previewWidth - margin);
    }

    if (top + previewHeight > window.innerHeight - margin) {
        top = Math.max(margin, pointerY - previewHeight - margin);
    }

    preview.style.left = `${left}px`;
    preview.style.top = `${top}px`;
}

function setupFileUploadAreas(scope = document) {
    if (!scope) return;

    scope.querySelectorAll('.file-upload-area').forEach(area => {
        if (area.dataset.fileUploadBound === '1') return;
        area.dataset.fileUploadBound = '1';

        const input = area.querySelector('input[type="file"]');
        const label = area.querySelector('.file-upload-label') || area.querySelector('span');
        if (!input || !label) return;

        if (!area.dataset.defaultLabel) {
            area.dataset.defaultLabel = (label.textContent || '').trim() || 'Upload';
        }

        let preview = area.querySelector('.file-upload-preview');
        if (!preview) {
            preview = document.createElement('div');
            preview.className = 'file-upload-preview';
            preview.innerHTML = '<img alt="Image preview">';
            area.appendChild(preview);
        }
        const previewImg = preview.querySelector('img');

        let clearBtn = area.querySelector('.file-upload-clear');
        if (!clearBtn) {
            clearBtn = document.createElement('button');
            clearBtn.type = 'button';
            clearBtn.className = 'file-upload-clear';
            clearBtn.setAttribute('aria-label', 'Clear selected file');
            clearBtn.textContent = 'X';
            area.appendChild(clearBtn);
        }

        const clearPreview = () => {
            const previousPreviewUrl = area.dataset.previewUrl;
            if (previousPreviewUrl) {
                URL.revokeObjectURL(previousPreviewUrl);
                delete area.dataset.previewUrl;
            }
            if (previewImg) previewImg.removeAttribute('src');
            area.classList.remove('has-preview');
            getFileUploadFloatingPreview().classList.remove('open');
        };

        const updateAreaState = (file) => {
            if (!file) {
                label.textContent = area.dataset.defaultLabel || 'Upload';
                area.classList.remove('has-file');
                clearPreview();
                return;
            }

            label.textContent = file.name;
            area.classList.add('has-file');
            clearPreview();

            if (previewImg && file.type && file.type.toLowerCase().startsWith('image/')) {
                const previewUrl = URL.createObjectURL(file);
                area.dataset.previewUrl = previewUrl;
                previewImg.src = previewUrl;
                area.classList.add('has-preview');
            }
        };

        clearBtn.addEventListener('click', event => {
            event.preventDefault();
            event.stopPropagation();
            input.value = '';
            updateAreaState(null);
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });

        const handleSelectedFile = (file) => {
            const validationError = validateSelectedImageFile(file);
            if (validationError) {
                input.value = '';
                input.setCustomValidity(validationError);
                updateAreaState(null);
                alert(validationError);
                return false;
            }

            input.setCustomValidity('');
            updateAreaState(file);
            return true;
        };

        const showFloatingPreview = event => {
            if (!previewImg?.src || !area.classList.contains('has-preview')) return;
            const floatingPreview = getFileUploadFloatingPreview();
            const floatingImg = floatingPreview.querySelector('img');
            if (!floatingImg) return;
            floatingImg.src = previewImg.src;
            floatingPreview.classList.add('open');
            positionFileUploadFloatingPreview(floatingPreview, event, area);
        };

        const hideFloatingPreview = () => {
            getFileUploadFloatingPreview().classList.remove('open');
        };

        area.addEventListener('mouseenter', showFloatingPreview);
        area.addEventListener('mousemove', showFloatingPreview);
        area.addEventListener('focusin', showFloatingPreview);
        area.addEventListener('mouseleave', hideFloatingPreview);
        area.addEventListener('focusout', hideFloatingPreview);

        input.addEventListener('change', () => {
            const file = input.files && input.files.length > 0 ? input.files[0] : null;
            handleSelectedFile(file);
        });

        const setDragActive = (isActive) => {
            area.classList.toggle('drag-over', isActive);
        };

        ['dragenter', 'dragover'].forEach(eventName => {
            area.addEventListener(eventName, event => {
                event.preventDefault();
                event.stopPropagation();
                setDragActive(true);
            });
        });

        ['dragleave', 'dragend'].forEach(eventName => {
            area.addEventListener(eventName, event => {
                event.preventDefault();
                event.stopPropagation();
                setDragActive(false);
            });
        });

        area.addEventListener('drop', event => {
            event.preventDefault();
            event.stopPropagation();
            setDragActive(false);

            const files = event.dataTransfer?.files;
            if (!files || files.length === 0) return;

            const droppedFile = files[0];
            if (!handleSelectedFile(droppedFile)) return;

            const dataTransfer = new DataTransfer();
            dataTransfer.items.add(droppedFile);
            input.files = dataTransfer.files;
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });

        updateAreaState(input.files && input.files.length > 0 ? input.files[0] : null);
    });
}

document.querySelectorAll('.modal-overlay').forEach(overlay => {
    overlay.addEventListener('click', e => { if (e.target === overlay) overlay.classList.remove('open'); });
});

let revealObserver = null;

function ensureRevealObserver() {
    if (prefersReducedMotion) return null;
    if (revealObserver) return revealObserver;

    revealObserver = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;
            entry.target.classList.add('is-visible');
            revealObserver?.unobserve(entry.target);
        });
    }, {
        threshold: 0.12,
        rootMargin: '0px 0px -8% 0px'
    });

    return revealObserver;
}

function revealAnimatedBlocks(scope = document) {
    if (prefersReducedMotion) return;
    if (!scope) return;

    const selectors = [
        '.page-actions',
        '.stat-card',
        '.dashboard-grid .card',
        '.slides-grid .slide-card',
        '.card > .data-table tbody tr',
        '.mini-table tbody tr',
        '.card-header',
        '.form-group'
    ];

    const observer = ensureRevealObserver();
    let staggerIndex = 0;

    selectors.forEach(selector => {
        scope.querySelectorAll(selector).forEach(node => {
            if (node.dataset.motionBound === '1') return;
            node.dataset.motionBound = '1';
            node.style.setProperty('--stagger-index', `${staggerIndex}`);
            node.classList.add('motion-reveal');
            observer?.observe(node);
            staggerIndex += 1;
        });
    });
}

function setupPageEntranceAnimation() {
    revealAnimatedBlocks(document);
    if (prefersReducedMotion) return;
    document.body.classList.add('page-enter');
    setTimeout(() => {
        document.body.classList.remove('page-enter');
    }, 380);
}

function setupSidebarPageTransitions() {
    if (prefersReducedMotion) return;

    let isNavigating = false;
    const freezeViewport = () => {
        const scrollTop = window.scrollY || document.documentElement.scrollTop || 0;
        const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
        document.body.style.position = 'fixed';
        document.body.style.top = `-${scrollTop}px`;
        document.body.style.left = '0';
        document.body.style.right = '0';
        document.body.style.width = '100%';
        document.body.style.overflow = 'hidden';
        if (scrollbarWidth > 0) {
            document.body.style.paddingRight = `${scrollbarWidth}px`;
        }
    };

    document.querySelectorAll('.sidebar-nav a.nav-item[href]').forEach(link => {
        link.addEventListener('click', e => {
            if (isNavigating) return;
            if (e.defaultPrevented || e.button !== 0 || e.metaKey || e.ctrlKey || e.shiftKey || e.altKey) return;
            if (link.target && link.target !== '_self') return;
            if (link.hasAttribute('download')) return;

            const nextUrl = new URL(link.href, window.location.href);
            if (nextUrl.origin !== window.location.origin) return;

            if (nextUrl.pathname === window.location.pathname && nextUrl.search === window.location.search && nextUrl.hash === window.location.hash) return;

            e.preventDefault();
            isNavigating = true;
            document.querySelectorAll('.sidebar-nav .nav-item.active').forEach(item => item.classList.remove('active'));
            link.classList.add('active');
            freezeViewport();
            document.body.classList.add('page-leaving');

            setTimeout(() => {
                window.location.assign(nextUrl.href);
            }, 260);
        });
    });
}

function animateSvgDraw(root) {
    if (prefersReducedMotion) return;
    if (!root) return;

    const geometricNodes = root.querySelectorAll('svg path, svg line, svg polyline, svg polygon, svg circle, svg rect, svg ellipse');
    let animatedCount = 0;

    geometricNodes.forEach(node => {
        if (typeof node.getTotalLength !== 'function') return;

        const styles = window.getComputedStyle(node);
        const stroke = (styles.stroke || '').trim().toLowerCase();
        if (!stroke || stroke === 'none') return;

        // Keep draw effect for line-style icons; skip fully filled shapes.
        const fill = (styles.fill || '').trim().toLowerCase();
        if (fill && fill !== 'none' && fill !== 'rgba(0, 0, 0, 0)') return;

        const length = node.getTotalLength();
        if (!Number.isFinite(length) || length <= 0) return;

        node.style.setProperty('--svg-path-len', `${length}`);
        node.style.animationDelay = `${Math.min(animatedCount * 35, 380)}ms`;
        node.classList.add('svg-draw-path');
        node.classList.remove('is-animated');

        // Reflow lets us restart draw animation when modal opens again.
        void node.getBoundingClientRect();

        node.classList.add('is-animated');
        animatedCount += 1;
    });
}

function animateNavItemSvg(navItem) {
    if (!navItem) return;
    if (navItem.dataset.svgAnimating === '1') return;
    navItem.dataset.svgAnimating = '1';
    animateSvgDraw(navItem);
    setTimeout(() => {
        navItem.dataset.svgAnimating = '0';
    }, 900);
}

function setupNavActiveSvgAnimation() {
    if (prefersReducedMotion) return;
    const activeItem = document.querySelector('.sidebar-nav .nav-item.active');
    animateNavItemSvg(activeItem);
}

function getRolePermission(name) {
    const value = document.body.getAttribute(`data-can-${name}`);
    if (value === null) return true;
    return value === 'true';
}

function ensureLockedAccessModal() {
    let modal = document.getElementById('lockedAccessModal');
    if (modal) return modal;

    const modalMarkup = `
<div class="modal-overlay" id="lockedAccessModal">
    <div class="modal modal-sm access-lock-modal">
        <div class="modal-body access-lock-body">
            <div class="access-lock-icon" aria-hidden="true">
                <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="3" y="11" width="18" height="11" rx="2"></rect>
                    <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
                </svg>
            </div>
            <h3>Access denied</h3>
            <p class="access-lock-text" id="lockedAccessText">You do not have permission to open this section.</p>
            <p class="access-lock-note">Access to this section is restricted by your role settings.</p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn-secondary" id="lockedAccessCloseBtn">Close</button>
        </div>
    </div>
</div>`;

    document.body.insertAdjacentHTML('beforeend', modalMarkup);
    modal = document.getElementById('lockedAccessModal');

    modal.addEventListener('click', e => {
        if (e.target === modal) modal.classList.remove('open');
    });

    document.getElementById('lockedAccessCloseBtn')?.addEventListener('click', () => {
        modal.classList.remove('open');
    });

    return modal;
}

function openLockedAccessModal(sectionName) {
    const modal = ensureLockedAccessModal();
    const text = document.getElementById('lockedAccessText');
    if (text) {
        const language = getAdminLanguage();
        const translatedSection = getAdminTranslation(sectionName, language);
        const messages = {
            az: `${translatedSection} bölməsini açmaq üçün icazəniz yoxdur.`,
            en: `You do not have permission to open ${sectionName}.`,
            ru: `У вас нет прав для открытия раздела ${translatedSection}.`
        };
        text.textContent = messages[language] || messages.en;
    }
    modal.classList.add('open');
}

function ensureRowDetailsModal() {
    let modal = document.getElementById('rowDetailsModal');
    if (modal) return modal;

    const modalMarkup = `
<div class="modal-overlay" id="rowDetailsModal">
    <div class="modal row-details-modal">
        <div class="modal-header">
            <h3 id="rowDetailsTitle">Details</h3>
            <button type="button" class="modal-close" id="rowDetailsCloseBtn">&#10005;</button>
        </div>
        <div class="modal-body">
            <div class="row-details-list" id="rowDetailsList"></div>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn-secondary" id="rowDetailsFooterCloseBtn">Close</button>
        </div>
    </div>
</div>`;

    document.body.insertAdjacentHTML('beforeend', modalMarkup);
    modal = document.getElementById('rowDetailsModal');

    const closeDetailsModal = () => modal.classList.remove('open');
    modal.addEventListener('click', e => {
        if (e.target === modal) closeDetailsModal();
    });
    document.getElementById('rowDetailsCloseBtn')?.addEventListener('click', closeDetailsModal);
    document.getElementById('rowDetailsFooterCloseBtn')?.addEventListener('click', closeDetailsModal);

    return modal;
}

function isMobileTableViewport() {
    return window.matchMedia?.('(max-width: 768px)').matches ?? window.innerWidth <= 768;
}

function isInteractiveTableTarget(target) {
    return Boolean(target?.closest('button, a, input, select, textarea, label, form, .actions-cell, .dropdown-wrapper, .file-upload-area'));
}

function getTableHeaders(table) {
    return Array.from(table.querySelectorAll('thead th')).map((th, index) => {
        const label = (th.textContent || '').replace(/\s+/g, ' ').trim();
        return label || `Column ${index + 1}`;
    });
}

function isTableActionCell(cell, label) {
    const normalizedLabel = (label || '').trim().toLowerCase();
    return normalizedLabel === 'actions'
        || cell.classList.contains('actions-cell')
        || Boolean(cell.querySelector(':scope > .actions-cell'));
}

function sanitizeDetailValue(cell) {
    const clone = cell.cloneNode(true);
    clone.querySelectorAll('script').forEach(node => node.remove());
    clone.querySelectorAll('[id]').forEach(node => node.removeAttribute('id'));
    clone.querySelectorAll('button, input, select, textarea').forEach(control => {
        control.disabled = true;
        control.removeAttribute('onclick');
    });
    clone.querySelectorAll('a').forEach(link => {
        link.removeAttribute('href');
        link.removeAttribute('onclick');
    });

    return clone.innerHTML.trim() || (clone.textContent || '').trim() || '-';
}

function getRowDetailsTitle(row) {
    const strongText = row.querySelector('strong')?.textContent?.replace(/\s+/g, ' ').trim();
    if (strongText) return strongText;

    const cells = Array.from(row.cells || []);
    const firstReadableCell = cells.find(cell => !cell.classList.contains('actions-cell') && (cell.textContent || '').trim());
    const title = firstReadableCell?.textContent?.replace(/\s+/g, ' ').trim();
    return title || 'Details';
}

function openRowDetails(row) {
    if (!row) return;

    const table = row.closest('table');
    if (!table) return;

    const headers = getTableHeaders(table);
    const items = Array.from(row.cells || [])
        .map((cell, index) => ({
            label: headers[index] || `Column ${index + 1}`,
            value: sanitizeDetailValue(cell),
            cell
        }))
        .filter(item => !isTableActionCell(item.cell, item.label));

    const modal = ensureRowDetailsModal();
    const title = document.getElementById('rowDetailsTitle');
    const list = document.getElementById('rowDetailsList');

    if (title) title.textContent = getRowDetailsTitle(row);
    if (list) {
        list.innerHTML = items.map(item => `
<div class="row-detail-item">
    <div class="row-detail-label">${item.label}</div>
    <div class="row-detail-value">${item.value}</div>
</div>`).join('');
    }

    modal.classList.add('open');
    revealAnimatedBlocks(modal);
}

function setupMobileTableDetails() {
    document.querySelectorAll('.data-table, .mini-table').forEach(table => {
        if (table.dataset.mobileDetailsTableBound !== '1') {
            table.dataset.mobileDetailsTableBound = '1';
            table.addEventListener('click', e => {
                if (e.mobileDetailsHandled) return;
                if (!isMobileTableViewport()) return;
                if (isInteractiveTableTarget(e.target)) return;

                const row = e.target.closest('tbody tr');
                if (!row || !table.contains(row)) return;
                if (window.getSelection?.().toString()) return;

                e.mobileDetailsHandled = true;
                openRowDetails(row);
            });
        }

        const rows = table.querySelectorAll('tbody tr');
        rows.forEach(row => {
            if (row.dataset.mobileDetailsBound === '1') return;
            row.dataset.mobileDetailsBound = '1';
            row.setAttribute('aria-label', 'Open row details');

            const hintCell = Array.from(row.cells || []).find(cell => !cell.classList.contains('actions-cell') && (cell.textContent || '').trim());
            if (hintCell && !hintCell.querySelector(':scope > .mobile-row-hint')) {
                const hint = document.createElement('span');
                hint.className = 'mobile-row-hint';
                hint.setAttribute('aria-hidden', 'true');
                hintCell.appendChild(hint);
            }

            row.addEventListener('click', e => {
                if (!isMobileTableViewport()) return;
                if (isInteractiveTableTarget(e.target)) return;
                if (window.getSelection?.().toString()) return;
                e.mobileDetailsHandled = true;
                openRowDetails(row);
            });

            row.addEventListener('keydown', e => {
                if (!isMobileTableViewport()) return;
                if (e.key !== 'Enter' && e.key !== ' ') return;
                if (isInteractiveTableTarget(e.target)) return;
                e.preventDefault();
                openRowDetails(row);
            });
        });
    });

    const syncRowTabIndex = () => {
        document.querySelectorAll('.data-table tbody tr, .mini-table tbody tr').forEach(row => {
            if (isMobileTableViewport()) {
                row.setAttribute('tabindex', '0');
            } else {
                row.removeAttribute('tabindex');
            }
        });
    };

    syncRowTabIndex();
    window.addEventListener('resize', syncRowTabIndex);
}

function lockNavigationByRole() {
    const permissionByPath = {
        '/admin/products': 'products',
        '/admin/carousel': 'slider',
        '/admin/authors': 'authors',
        '/admin/genres': 'genres',
        '/admin/users': 'users',
        '/admin/roles': 'roles'
    };

    const lockIconMarkup = `
<svg class="lock-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
    <rect x="3" y="11" width="18" height="11" rx="2"></rect>
    <path d="M7 11V7a5 5 0 0 1 10 0v4"></path>
</svg>`;

    document.querySelectorAll('.sidebar-nav a.nav-item[href]').forEach(link => {
        const pathname = new URL(link.getAttribute('href'), window.location.origin).pathname.toLowerCase();
        const permissionKey = permissionByPath[pathname];
        if (!permissionKey) return;
        if (getRolePermission(permissionKey)) return;

        const sectionName = link.querySelector('span')?.textContent?.trim() || 'this section';
        const locked = document.createElement('span');
        locked.className = `${link.className} nav-locked`;
        locked.setAttribute('role', 'button');
        locked.setAttribute('tabindex', '0');
        locked.setAttribute('aria-label', `Access denied for ${sectionName}`);
        locked.dataset.section = sectionName;
        locked.innerHTML = `${link.innerHTML}${lockIconMarkup}`;
        link.replaceWith(locked);
    });

    document.querySelectorAll('.sidebar-nav .nav-locked').forEach(item => {
        if (!item.hasAttribute('role')) item.setAttribute('role', 'button');
        if (!item.hasAttribute('tabindex')) item.setAttribute('tabindex', '0');

        if (!item.dataset.section) {
            const label = item.querySelector('span')?.textContent?.trim();
            if (label) item.dataset.section = label;
        }

        item.addEventListener('click', () => {
            openLockedAccessModal(item.dataset.section || 'this section');
        });

        item.addEventListener('keydown', e => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                openLockedAccessModal(item.dataset.section || 'this section');
            }
        });
    });
}

lockNavigationByRole();
setupFileUploadAreas();
setupMobileTableDetails();
setupPageEntranceAnimation();
setupSidebarPageTransitions();
setupNavActiveSvgAnimation();
applyAdminLanguage(getAdminLanguage());
observeAdminLanguageChanges();
