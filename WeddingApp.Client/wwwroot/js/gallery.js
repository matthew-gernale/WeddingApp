function setBodyScrollLock(locked) {
    document.body.style.overflow = locked ? 'hidden' : '';
}

function scrollToSection(id) {
    const section = document.getElementById(id);

    if (section) {
        section.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
}

async function startGalleryCamera(video, facingMode) {
    if (!navigator.mediaDevices?.getUserMedia) return false;

    stopGalleryCamera(video);
    try {
        video.srcObject = await navigator.mediaDevices.getUserMedia({ video: { facingMode: { ideal: facingMode } }, audio: false });
        await video.play();
        return true;
    } catch {
        stopGalleryCamera(video);
        return false;
    }
}

async function switchGalleryCamera(video, facingMode, previousMode) {
    stopGalleryCamera(video);

    try {
        video.srcObject = await navigator.mediaDevices.getUserMedia({ video: { facingMode: { exact: facingMode } }, audio: false });
        await video.play();
        return true;
    } catch {
        await startGalleryCamera(video, previousMode);
        return false;
    }
}

function stopGalleryCamera(video) {
    video.srcObject?.getTracks().forEach(track => track.stop());
    video.srcObject = null;
}

function captureGalleryPhoto(video, mirror) {
    if (!video.videoWidth || !video.videoHeight) throw new Error('Camera is not ready');

    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const context = canvas.getContext('2d');
    if (mirror) {
        context.translate(canvas.width, 0);
        context.scale(-1, 1);
    }
    context.drawImage(video, 0, 0);
    return canvas.toDataURL('image/jpeg', 0.9);
}
