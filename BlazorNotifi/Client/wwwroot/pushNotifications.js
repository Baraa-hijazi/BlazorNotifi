; (function () {
    // Note: Replace with your own key pair before deploying
    const applicationServerPublicKey = "BDX8x7dX4jaALPG94e0vSYnbbnlRL4_ktiANFe9-FzyP2JzjRVGqQR33NXSk5oFLi8qLyYT_Dx4mM_KfsrkvG-8";

    window.blazorPushNotifications = {
        requestSubscription: async () => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (!existingSubscription) {
                const newSubscription = await subscribe(worker);
                if (newSubscription) {
                    return {
                        url: newSubscription.endpoint,
                        p256dh: arrayBufferToBase64(newSubscription.getKey("p256dh")),
                        auth: arrayBufferToBase64(newSubscription.getKey("auth"))
                    };
                }
            }
        },

        unSubscribe: async () => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();
            if (existingSubscription) {
                existingSubscription.unsubscribe();
                return true;
            }
        },
    };

    async function subscribe(worker) {
        try {
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey,
            });
        } catch (error) {
            if (error.name === "NotAllowedError") {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = "";
        const bytes = new Uint8Array(buffer);
        const len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
})();