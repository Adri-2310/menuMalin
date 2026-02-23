// ============================================
// Gestion du thème (dark/light)
// ============================================

/**
 * Applique le thème au document
 * @param {string} theme - "light" ou "dark"
 */
window.applyTheme = function (theme) {
    const html = document.documentElement;

    if (theme === 'dark') {
        html.classList.add('dark-mode');
        html.classList.remove('light-mode');
    } else {
        html.classList.add('light-mode');
        html.classList.remove('dark-mode');
    }

    console.log(`Thème appliqué: ${theme}`);
};

/**
 * Initialise le thème au chargement de la page
 */
window.initializeTheme = function (defaultTheme) {
    const savedTheme = localStorage.getItem('user:theme') || defaultTheme || 'light';
    window.applyTheme(savedTheme);
};

// ============================================
// Utilitaires pour localStorage
// ============================================

/**
 * Récupère une valeur du localStorage
 */
window.getLocalStorage = function (key) {
    return localStorage.getItem(key);
};

/**
 * Sauvegarde une valeur dans localStorage
 */
window.setLocalStorage = function (key, value) {
    localStorage.setItem(key, value);
};

/**
 * Supprime une valeur du localStorage
 */
window.removeLocalStorage = function (key) {
    localStorage.removeItem(key);
};

// Initialiser le thème au chargement
document.addEventListener('DOMContentLoaded', function () {
    window.initializeTheme('light');
});
