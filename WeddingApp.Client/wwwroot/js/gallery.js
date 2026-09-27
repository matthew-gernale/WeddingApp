function setBodyScrollLock(locked) {
    document.body.style.overflow = locked ? 'hidden' : '';
}

function scrollToSection(id) {
    const section = document.getElementById(id);

    if (section) {
        section.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}
