window.hideSplash = () => {
    const splash = document.getElementById('splash_screen');
    if (splash) {
        setTimeout(() => {
            splash.classList.add('splash-hide');
            setTimeout(() => {
                splash.style.display = 'none';
            }, 600); // Espera a que acabe la transición
        }, 1200); // Espera inicial antes de empezar la animación (puedes poner 0 si quieres que sea instantáneo)
    }
}

function changetoggler() {
    const toggler = document.getElementById('navbar-toggler');
    if (toggler) toggler.checked = false;
}

function downloadJsonFile(fileName, jsonContent) {
    const blob = new Blob([jsonContent], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    a.click();
    URL.revokeObjectURL(url);
}

window.clientErrorTap = {
    init: function (dotnetRef) {
        window.addEventListener('error', function (e) {
            try {
                dotnetRef.invokeMethodAsync('ReportClientError',
                    `${e.message} @ ${e.filename}:${e.lineno}:${e.colno}`);
            } catch { }
        });

        window.addEventListener('unhandledrejection', function (e) {
            try {
                const msg = (e.reason && e.reason.stack) ? e.reason.stack : (e.reason || 'unhandledrejection');
                dotnetRef.invokeMethodAsync('ReportClientError', msg.toString());
            } catch { }
        });
    }
};

function scrollToTop() {
    var x = document.getElementById("main-ariticle");
    x.scrollTo({ top: 0, behavior: 'smooth' });
}

function goBack() {
    window.history.back();
}
function replaceUrl(url) {
    window.history.replaceState({}, "", url);
}