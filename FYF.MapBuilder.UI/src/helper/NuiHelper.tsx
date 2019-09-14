declare type NuiCallback = (type: string, data: any) => void;

export function ReceiveNuiMessage(messageType: string, callback: NuiCallback) {
    window.addEventListener("message", (event) => {
        if (event.data.messageType === messageType) {
            if (callback && typeof(callback) === "function") {
                callback(messageType, event.data);
            }
        }
    });
}

export function SendNuiMessage(eventName: string, data: any) {

    const endpoint = `http://fyf-mapbuilder/${eventName}`;
    const payload = JSON.stringify(data);
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
    }

    fetch(endpoint, {
        method: 'POST',
        headers: headers,
        body: payload
    });
}