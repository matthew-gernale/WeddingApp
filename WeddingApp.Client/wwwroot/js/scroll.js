function scrollToFilter(selector) {
    const button = document.querySelector(selector);

    if (button) {
        button.scrollIntoView({
            behavior: 'smooth',
            block: 'nearest',
            inline: 'center'
        });
    }
}