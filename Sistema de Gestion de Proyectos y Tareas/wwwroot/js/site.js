// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    // Dark mode toggle
    const body = document.body;
    const darkToggleId = 'darkModeToggle';

    function applyDarkMode(enabled) {
        if (enabled) body.classList.add('dark-mode');
        else body.classList.remove('dark-mode');
    }

    // Initialize from localStorage
    const enabled = localStorage.getItem('darkMode') === 'true';
    applyDarkMode(enabled);

    // Expose toggle function
    window.toggleDarkMode = function () {
        const current = body.classList.contains('dark-mode');
        applyDarkMode(!current);
        localStorage.setItem('darkMode', (!current).toString());
    };

    // Modal helper: show a bootstrap modal with title and body
    window.showModal = function (title, bodyHtml) {
        let modal = document.getElementById('appGenericModal');
        if (!modal) {
            modal = document.createElement('div');
            modal.className = 'modal fade';
            modal.id = 'appGenericModal';
            modal.tabIndex = -1;
            modal.setAttribute('aria-hidden', 'true');
            modal.innerHTML = "<div class='modal-dialog modal-dialog-centered'><div class='modal-content'><div class='modal-header'><h5 class='modal-title' id='appGenericModalTitle'></h5><button type='button' class='btn-close' data-bs-dismiss='modal' aria-label='Cerrar'></button></div><div class='modal-body' id='appGenericModalBody'></div><div class='modal-footer'><button type='button' class='btn btn-secondary' data-bs-dismiss='modal'>Cerrar</button></div></div></div>";
            document.body.appendChild(modal);
        }
        document.getElementById('appGenericModalTitle').innerText = title;
        document.getElementById('appGenericModalBody').innerHTML = bodyHtml;
        var bsModal = new bootstrap.Modal(modal);
        bsModal.show();
    };

    // Normalize inputs on blur
    document.addEventListener('blur', function (e) {
        var target = e.target;
        if (target && (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA')) {
            if (target.type === 'text' || target.type === 'search' || target.type === 'password' || target.tagName === 'TEXTAREA') {
                var v = target.value;
                if (v != null) {
                    // collapse multiple spaces and trim
                    v = v.replace(/\s+/g, ' ').trim();
                    target.value = v;
                }
            }
        }
    }, true);
})();
