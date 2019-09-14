function ReceiveNuiMessage(messageType, callback) {
    window.addEventListener("message", function (event) {
        if (event.data.messageType === messageType) {
            if (callback && typeof callback === "function") {
                callback(messageType, event.data);
            }
        }
    });
}
function SendNuiMessage(resource, event, data) {
    var endpoint = "http://" + resource + "/" + event;
    var payload = JSON.stringify(data);
    var headers = {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    };
    fetch(endpoint, {
        method: 'POST',
        headers: headers,
        body: payload
    });
}
//# sourceMappingURL=NuiHelper.js.map