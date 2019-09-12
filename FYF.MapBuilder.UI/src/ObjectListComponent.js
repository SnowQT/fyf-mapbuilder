"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const JsonObjectsList = require("./assets/meta/objects.json");
class ObjectListComponent extends React.Component {
    createSelectorForObjectVariants(fields) {
        const selectOptions = fields.map(field => React.createElement("option", { value: field }, field));
        return (React.createElement("select", null, selectOptions));
    }
    render() {
        const listItemsObject = JsonObjectsList.map(obj => {
            return (React.createElement("li", { key: obj.name },
                obj.name,
                this.createSelectorForObjectVariants(obj.variants)));
        });
        return (React.createElement("ul", null, listItemsObject));
    }
}
exports.default = ObjectListComponent;
//# sourceMappingURL=ObjectListComponent.js.map