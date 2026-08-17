export function moveSlider() {

    const link = document.querySelector(".nav-link.active");
    const slider = document.querySelector(".slider");

    if (!link || !slider)
        return;

    slider.style.width = `${link.offsetWidth}px`;
    slider.style.transform = `translateX(${link.offsetLeft - 5}px)`;
}