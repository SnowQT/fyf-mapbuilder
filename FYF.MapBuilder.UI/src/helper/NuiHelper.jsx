export function ReceiveNuiMessage(messageType, callback) {
    window.addEventListener("message", (event) => {
        const message = event.data;

        //Exempt toggle messages.
        if (message.messageType === "toggleInit") {
            return;
        }

        if (message.messageType === messageType) {
            if (callback && typeof (callback) === "function") {
                callback(message);
            }
        }
    });
}

export function ReceiveToggle(openCb, closeCb) {
    window.addEventListener("message", (event) => {
        const message = event.data;

        if (message.messageType === "toggleInit") {
            let key = parseInt(message.toggleKey);
            let cbName = message.toggleName;

            openCb();

            window.onkeydown = function (e) {
                var pressedKey = parseInt(e.keyCode ? e.keyCode : e.which);
                if (key === pressedKey) {
                    SendNuiMessage("toggleInvoke_" + cbName, {});
                    closeCb();
                }
            }

            return;
        }
    });
}

export function SendNuiMessage(eventName, data) {
    const endpoint = `http://fyf-mapbuilder/${eventName}`;
    const payload = JSON.stringify(data);
    const headers =
    {
        "Accept": "application/json",
        "Content-Type": "application/json",
    }

    fetch(endpoint, {
        method: 'POST',
        headers: headers,
        body: payload
    });
}