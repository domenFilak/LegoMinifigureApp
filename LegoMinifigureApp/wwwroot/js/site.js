//converts every <time data-local-time datetime="...UTC..."> to visitor's local time
document.querySelectorAll('time[data-local-time]').forEach(el => {
    const date = new Date(el.getAttribute('datetime'));
    if (isNaN(date)) {
        return; //leave server's fallback text if the value can't be read
    }

    el.textContent = date.toLocaleString([], {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
});