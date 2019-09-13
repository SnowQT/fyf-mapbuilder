"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const JsonObjectsList = require("./assets/meta/objects.json");
class ObjectListItem {
    constructor(name, image, variants, category, tags) {
        this.name = name;
        this.image = image;
        this.variants = variants;
        this.category = category;
        this.tags = tags;
    }
}
class ObjectListComponent extends React.Component {
    constructor(props) {
        super(props);
        this.maxSelectSize = 21;
        this.OnObjectSelected = (event) => {
            let objectName = event.target.value;
            let foundObject = JsonObjectsList.find(obj => obj.name == objectName);
            if (foundObject == undefined) {
                console.log("Unknown selected object.");
                return;
            }
            const objectListItem = new ObjectListItem(foundObject.name, foundObject.image, foundObject.variants, foundObject.category, foundObject.tags);
            this.setState({ currentSelectedObject: objectListItem });
        };
        this.OnObjectVariantChanged = (event) => {
            console.log(event.target.value);
        };
        const objectListItem = new ObjectListItem("unknown", "", [], [], []);
        this.state = { currentSelectedObject: objectListItem };
    }
    render() {
        const { currentSelectedObject } = this.state;
        const currentObjectVariantsSelect = currentSelectedObject.variants.map(variant => {
            return React.createElement("option", { value: variant }, variant);
        });
        const listItemsObject = JsonObjectsList.map(obj => {
            return (React.createElement("option", { value: obj.name },
                " ",
                obj.name,
                " "));
        });
        return (React.createElement("div", { id: "object-list" },
            React.createElement("h3", null, "Objects"),
            React.createElement("select", { name: "objects", onChange: this.OnObjectSelected, size: this.maxSelectSize }, listItemsObject),
            React.createElement("h3", null, "Object properties"),
            React.createElement("div", null,
                React.createElement("h5", null,
                    "Name: ",
                    currentSelectedObject.name),
                React.createElement("h5", null, "Variants:"),
                React.createElement("select", { name: "variants", onChange: this.OnObjectVariantChanged }, currentObjectVariantsSelect))));
    }
}
exports.default = ObjectListComponent;
//# sourceMappingURL=ObjectListComponent.js.map