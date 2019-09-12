"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const Data = require("./assets/meta/objects.json");
class ObjectListComponent extends React.Component {
    componentDidMount() {
        console.log('I was triggered during componentDidMount, DATA: ' + JSON.stringify(Data));
    }
    render() {
        //console.log("Data is: " + Data);
        return (React.createElement("ul", null,
            React.createElement("li", { key: "life" }, "Awesome!")));
    }
}
exports.default = ObjectListComponent;
//# sourceMappingURL=ObjectListComponent.js.map