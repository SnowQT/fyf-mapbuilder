import * as React from "react";
import * as JsonObjectsList from "./assets/meta/objects.json"

export default class ObjectListComponent extends React.Component {

    createSelectorForObjectVariants(fields) {
        const selectOptions = fields.map(
            field => <option value={field}>{field}</option>
        );

        return (
            <select>
                {selectOptions}
            </select>
        );
    }

    render() {

        const listItemsObject = JsonObjectsList.map(obj => {
                return (
                    <li key={obj.name}>
                        {obj.name}
                        { this.createSelectorForObjectVariants(obj.variants) }
                    </li>
                )
        });

        return (
            <ul>
                { listItemsObject }
            </ul>
        );
    }
}