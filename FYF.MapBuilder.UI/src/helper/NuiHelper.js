"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function ReceiveNuiMessage(messageType, callback) {
    window.addEventListener("message", (event) => {
        if (event.data.messageType === messageType) {
            if (callback && typeof (callback) === "function") {
                callback(messageType, event.data);
            }
        }
    });
}
exports.ReceiveNuiMessage = ReceiveNuiMessage;
function SendNuiMessage(eventName, data) {
    const endpoint = `http://fyf-mapbuilder/${eventName}`;
    const payload = JSON.stringify(data);
    const headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
    };
    fetch(endpoint, {
        method: 'POST',
        headers: headers,
        body: payload
    });
}
exports.SendNuiMessage = SendNuiMessage;
//# sourceMappingURL=NuiHelper.js.map