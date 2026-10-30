//light/dark/auto theme mode. Loaded in <head> so theme is set before the page is drawn
(() => {
    const storageKey = 'theme';
    const choices = ['light', 'dark', 'auto'];
    const next = { light: 'dark', dark: 'auto', auto: 'light' };
    const labels = { light: 'Light', dark: 'Dark', auto: 'Auto' };
    const icons = { light: '☀', dark: '☾', auto: '◐' };
    const systemDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');

    function readChoice() {
        try {
            const stored = localStorage.getItem(storageKey);
            return choices.includes(stored) ? stored : 'auto';
        } catch {
            return 'auto'; //if storage is blocked
        }
    }

    function saveChoice(value) {
        try {
            localStorage.setItem(storageKey, value);
        } catch {
            //not saved; theme still works for this page
        }
    }

    function apply(value) {
        const isDark = value === 'dark' || (value === 'auto' && systemDarkQuery.matches);
        document.documentElement.setAttribute('data-bs-theme', isDark ? 'dark' : 'light');
    }

    let choice = readChoice();
    apply(choice);

    //if auto mode, follow system when it switches
    systemDarkQuery.addEventListener('change', () => {
        if (choice === 'auto') {
            apply(choice);
        }
    });

    //toggle buttons once the pages HTML exists
    document.addEventListener('DOMContentLoaded', () => {
        document.querySelectorAll('[data-theme-toggle]').forEach(button => {
            const render = () => {
                button.querySelector('[data-theme-icon]').textContent = icons[choice];
                button.querySelector('[data-theme-label]').textContent = labels[choice];
                button.setAttribute('aria-label', `Theme: ${labels[choice]}. Click to change.`);
            };

            render();

            button.addEventListener('click', () => {
                choice = next[choice];
                saveChoice(choice);
                apply(choice);
                render();
            });
        });
    });
})();