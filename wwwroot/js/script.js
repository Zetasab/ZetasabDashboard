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