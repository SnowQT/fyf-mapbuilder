"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const React = require("react");
const JsonObjectsList = require("./assets/meta/objects.json");
const NuiHelper_1 = require("./helper/NuiHelper");
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
            let foundObject = JsonObjectsList.find(obj => obj.name === objectName);
            if (foundObject == undefined) {
                console.log("Unknown selected object.");
                return;
            }
            const objectListItem = new ObjectListItem(foundObject.name, foundObject.image, foundObject.variants, foundObject.category, foundObject.tags);
            this.setState({ currentSelectedObject: objectListItem }, () => {
                this.OnObjectChanged("");
            });
        };
        this.OnObjectVariantChanged = (event) => {
            this.OnObjectChanged(event.target.value);
        };
        const objectListItem = new ObjectListItem("unknown", "", [], [], []);
        this.state = { currentSelectedObject: objectListItem };
    }
    OnObjectChanged(variant) {
        const { currentSelectedObject } = this.state;
        let objectName = `${currentSelectedObject}`;
        //No variant was supplied, pick the first one.
        if (variant === "") {
            //We have atleast one variant we can pick from.
            //  If we have no variants, we want to use the name of the current object without any variant.
            if (currentSelectedObject.variants.length > 0) {
                let firstVariant = currentSelectedObject.variants[0];
                objectName += `_${firstVariant}`;
            }
        }
        //Else we us the variant we got supplied.
        else {
            objectName += `_${variant}`;
        }
        //Send a message back to the client.
        NuiHelper_1.SendNuiMessage("ObjectChanged", {
            name: objectName
        });
    }
    render() {
        const { currentSelectedObject } = this.state;
        const currentObjectVariantsSelect = currentSelectedObject.variants.map(variant => {
            return React.createElement("option", { key: variant, value: variant }, variant);
        });
        const listItemsObject = JsonObjectsList.map(obj => {
            return (React.createElement("option", { key: obj.name, value: obj.name }, obj.name));
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