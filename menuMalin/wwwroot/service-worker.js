// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', (event) => {
    // En développement, ne pas intercepter — laisser passer au réseau
    // Cela évite l'avertissement "no-op fetch handler"
    event.respondWith(fetch(event.request));
});
