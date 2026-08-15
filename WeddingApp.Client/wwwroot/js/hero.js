let lastScrollY = 0;

window.initWeddingHero = () => {
    const hero = document.getElementById("wedding-hero");

    if (!hero) return;

    const handleScroll = () => {
        const currentScrollY = window.scrollY;

        // At the very top: always show the hero
        if (currentScrollY <= 20) {
            hero.classList.remove("hero-hidden");
            lastScrollY = currentScrollY;
            return;
        }

        // Scrolling down
        if (currentScrollY > lastScrollY) {
            hero.classList.add("hero-hidden");
        }

        // Scrolling up
        if (currentScrollY < lastScrollY) {
            hero.classList.remove("hero-hidden");
        }

        lastScrollY = currentScrollY;
    };

    window.addEventListener("scroll", handleScroll, {
        passive: true
    });
};