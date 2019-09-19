import * as React from "react";
import { SendNuiMessage } from "../helper/NuiHelper.jsx";
import JsonObjectData from "../assets/metadata/objects.json";

import styles from "../assets/css/objectlist.css"
import global from "../assets/css/global.css"

class ObjectListItem {
    constructor(name, image, variants, category, tags) {
        this.name = name;
        this.image = image;
        this.variants = variants;
        this.currentObject = "";
        this.category = category;
        this.tags = tags;
    }
}

class ObjectBrowser extends React.Component  {
    constructor(props) {
        super(props);

        this.maxItemsObjects = 20;
        this.maxItemsVariants = 10;

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

        //Show the current selected objects name.
        currentSelectedObject.currentObject = objectName;
        this.setState({ currentSelectedObject: currentSelectedObject });

        //Send a message back to the client.
        SendNuiMessage("OnObjectChanged", {
            name: objectName
        });
    }

    OnObjectSelected(event) {
        //Try finding it in the json file.
        let objectName = event.target.value;
        let foundObject = JsonObjectData.find(obj => obj.name === objectName);
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
        const currentItem = this.state.currentSelectedObject;

        //Map
        const currentObjectVariantsSelect = currentItem.variants.map(variant => {
            return <option key={variant} value={variant}>{variant}</option>
        });

        const listItemsObject = JsonObjectData.map(obj => {
            return (
                <option key={obj.name} value={obj.name}>{obj.name}</option>
            )
        });

        //Bind the "this" context to the callbacks.
        const CbObjectChanged = this.OnObjectSelected.bind(this);
        const CbObjectVariantChanged = this.OnObjectVariantChanged.bind(this);

        return (
            <div className={styles.container}>
                <div className={styles.container_inner}>
                    <h2 className={styles.title}>OBJECT BROWSER</h2>
                    <h3 className={styles.tltle}>Objects:</h3>

                    <select
                        name="objects" onChange={CbObjectChanged}
                        size={this.maxItemsObjects} className={global.w100}
                    >
                        { listItemsObject }
                    </select>

                    <h3 className={styles.title}>Properties</h3>
                    <div>
                        <h5 className={styles.field}>Name: {currentItem.name}</h5>
                        <h5 className={styles.field}>Full name: {currentItem.currentObject}</h5>
                        <h3 className={styles.title}>Variants:</h3>

                        <select
                            name="variants" onChange={CbObjectVariantChanged}
                            size={this.maxItemsVariants} className={global.w100}
                        >
                            { currentObjectVariantsSelect }
                        </select>

                    </div>
                </div>
            </div>
        );
    }
}

export default ObjectBrowser;