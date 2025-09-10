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

// one timer per image id
const timers = new Map();

/**
 * Start rotating `photos` (array of strings) every 2s on the <img id="{id}">
 */
function startTimer(id, photos) {
    const el = document.getElementById(id);
    if (!el || !Array.isArray(photos) || photos.length < 2) return;
    if (timers.has(id)) return; // already running

    let index = 1; // start from second photo (first is already shown)
    const handle = setInterval(() => {
        console.log("timer " + index)
        el.src = photos[index];
        index = (index + 1) % photos.length; // wrap
    }, 850);

    timers.set(id, handle);
}

/**
 * Stop the timer for this image and reset to the given `photo` (usually photos[0])
 */
function stopTimer(id, photo) {
    const handle = timers.get(id);
    if (handle) {
        clearInterval(handle);
        timers.delete(id);
    }
    const el = document.getElementById(id);
    if (el && photo) el.src = photo; // reset
}

function replaceQuery(paramsObj) {
    const url = new URL(window.location.href);

    // 👇 Limpia toda la query actual
    url.search = "";

    // Añade solo los params que recibes
    for (const [k, v] of Object.entries(paramsObj)) {
        if (v) url.searchParams.set(k, v);
    }

    window.history.replaceState({}, "", url.toString());
}