import * as React from "react";
import { SendNuiMessage } from "../helper/NuiHelper.jsx";
import JsonObjectsList from "../assets/metadata/objects.json";
//import style from "./assets/css/objectlist.module.css"

class ObjectListItem {
    constructor(name, image, variants, category, tags) {
        this.name = name;
        this.image = image;
        this.variants = variants;
        this.category = category;
        this.tags = tags;
    }
}

class ObjectListComponent extends React.Component  {
    constructor(props) {
        super(props);

        this.maxItems = 20;

        this.state = {
            currentSelectedObject: new ObjectListItem("unknown", "", [], [], [])
        };
    }

    OnObjectChanged(variant) {
        const { currentSelectedObject } = this.state;
        let objectName = `${currentSelectedObject.name}`;

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
        SendNuiMessage("OnObjectChanged", {
            name: objectName
        });
    }

    OnObjectSelected(event) {
        //Try finding it in the json file.
        let objectName = event.target.value;
        let foundObject = JsonObjectsList.find(obj => obj.name === objectName);
        if (foundObject == undefined) {
            console.log(`Could not find ${objectName} in the object metadata.`)
            return;
        }

        //Create a new currently selected object.
        const objectListItem = new ObjectListItem(
            foundObject.name, foundObject.image, foundObject.variants,
            foundObject.category, foundObject.tags
        );

        //Change the state, let game code know we changed objects.
        this.setState({ currentSelectedObject: objectListItem }, () => {
            this.OnObjectChanged("");
        });
    }

    OnObjectVariantChanged(event) {
        this.OnObjectChanged(event.target.value);
    }

    render() {
        const { currentSelectedObject } = this.state;

        //Map
        const currentObjectVariantsSelect = currentSelectedObject.variants.map(variant => {
            return <option key={variant} value={variant}>{variant}</option>
        });

        const listItemsObject = JsonObjectsList.map(obj => {
            return (
                <option key={obj.name} value={obj.name}>{obj.name}</option>
            )
        });

        return (
            <div id="object-list">
                <h3>Objects</h3>
                <select name="objects" onChange={this.OnObjectSelected} size={this.maxSelectSize}>
                    { listItemsObject }
                </select>

                <h3>Object properties</h3>
                <div>
                    <h5>Name: {currentSelectedObject.name}</h5>
                    <h5>Variants:</h5>
                    <select name="variants" onChange={this.OnObjectVariantChanged}>
                        { currentObjectVariantsSelect }
                    </select>
                </div>
            </div>
        );
    }
}

export default ObjectListComponent;