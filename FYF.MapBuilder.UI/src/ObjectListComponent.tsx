import * as React from "react";
import * as JsonObjectsList from "./assets/meta/objects.json";
import { SendNuiMessage } from "./helper/NuiHelper";

export interface ObjectListProps {
}

interface ObjectListState {
    currentSelectedObject: ObjectListItem;
}

class ObjectListItem {
    name: string;
    image: string;
    variants: Array<string>;
    category: Array<string>;
    tags: Array<string>;

    constructor(name : string, image: string, variants : Array<string>, category : Array<string>, tags : Array<string>) {
        this.name = name;
        this.image = image;
        this.variants = variants;
        this.category = category;
        this.tags = tags;
    }
}

class ObjectListComponent extends React.Component<ObjectListProps, ObjectListState>  {

    maxSelectSize: number = 21;

    constructor(props: ObjectListProps) {
        super(props);

        const objectListItem = new ObjectListItem("unknown", "", [], [], [])
        this.state = { currentSelectedObject: objectListItem };
    }

    OnObjectChanged(variant: string) {
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
        SendNuiMessage("ObjectChanged", {
            name: objectName
        });
    }

    OnObjectSelected = (event) => {
        let objectName: string = event.target.value;
        let foundObject = JsonObjectsList.find(obj => obj.name === objectName);

        if (foundObject == undefined) {
            console.log("Unknown selected object.")
            return;
        }

        const objectListItem = new ObjectListItem(
            foundObject.name, foundObject.image, foundObject.variants,
            foundObject.category, foundObject.tags
        );

        this.setState({ currentSelectedObject: objectListItem }, () => {
            this.OnObjectChanged("");
        });
    }

    OnObjectVariantChanged = (event) => {
        this.OnObjectChanged(event.target.value);
    }

    render(): any {
        const { currentSelectedObject } = this.state;

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