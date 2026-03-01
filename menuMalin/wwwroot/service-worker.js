// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
self.addEventListener('fetch', (event) => {
    // En développement, ne pas intercepter — laisser passer au réseau
    // Cela évite l'avertissement "no-op fetch handler"
    // Ignorer les erreurs 404 sur les fichiers .pdb (symboles de debug)
    event.respondWith(
        fetch(event.request).catch((error) => {
            // Si c'est un fichier .pdb, retourner une réponse vide au lieu de lever une erreur
            if (event.request.url.includes('.pdb')) {
                return new Response('', { status: 404, statusText: 'Not Found' });
            }
            throw error;
        })
    );
});
