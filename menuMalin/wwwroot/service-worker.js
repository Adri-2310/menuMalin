// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', (event) => {
    // Ignorer silencieusement les requêtes pour les fichiers .pdb (symboles de debug)
    // Ces fichiers ne sont pas nécessaires en développement
    // Retourner 200 au lieu de 404 pour éviter que le framework Blazor les traite comme une erreur
    if (event.request.url.includes('.pdb')) {
        event.respondWith(
            new Response('', { status: 200, statusText: 'OK', headers: { 'Content-Type': 'application/octet-stream' } })
        );
        return;
    }

    // Pour les autres requêtes, laisser passer au réseau
    event.respondWith(
        fetch(event.request).catch((error) => {
            console.warn('Fetch failed for:', event.request.url, error);
            throw error;
        })
    );
});
