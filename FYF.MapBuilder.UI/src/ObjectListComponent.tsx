import * as React from "react";
import * as JsonObjectsList from "./assets/meta/objects.json"

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

    OnObjectSelected = (event) => {
        let objectName: string = event.target.value;
        let foundObject = JsonObjectsList.find(obj => obj.name == objectName);

        if (foundObject == undefined) {
            console.log("Unknown selected object.")
            return;
        }

        const objectListItem = new ObjectListItem(
            foundObject.name, foundObject.image, foundObject.variants,
            foundObject.category, foundObject.tags
        );

        this.setState({ currentSelectedObject: objectListItem });
    }

    OnObjectVariantChanged = (event) => {
        console.log(event.target.value);
    }

    render(): any {
        const { currentSelectedObject } = this.state;

        const currentObjectVariantsSelect = currentSelectedObject.variants.map(variant => {
            return <option value={variant}>{variant}</option>
        });

        const listItemsObject = JsonObjectsList.map(obj => {
            return (
                <option value={obj.name}> {obj.name} </option>
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