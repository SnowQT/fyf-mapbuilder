declare type NuiCallback = (data: any) => void;

export function ReceiveNuiMessage(messageType: string, callback: NuiCallback) {
    window.addEventListener("message", (event) => {
        const message = event.data;

        //Exempt toggle messages.
        if (message.messageType === "toggleInit") {
            HandleToggle(message);
            return;
        }

        if (message.messageType === messageType) {
            if (callback && typeof(callback) === "function") {
                callback(message);
            }
        }
    });
}

function HandleToggle(data : any) {
    let key = parseInt(data.toggleKey);
    let cbName = data.toggleName;

    window.onkeydown = function (e : any) {
        var pressedKey = parseInt(e.keyCode ? e.keyCode : e.which);
        if (key === pressedKey) {
            SendNuiMessage("toggleInvoke_" + cbName, {});
        }
    }
}

export function SendNuiMessage(eventName: string, data: any) {
    console.log("Sending shit back to " + eventName + " with: " + JSON.stringify(data));
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