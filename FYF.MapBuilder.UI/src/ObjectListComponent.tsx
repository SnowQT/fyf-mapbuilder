import * as React from "react";
import * as Data from "./assets/meta/objects.json"

export default class ObjectListComponent extends React.Component {
    componentDidMount() {
        console.log('I was triggered during componentDidMount, DATA: ' + JSON.stringify(Data));
    }

    render() {
        //console.log("Data is: " + Data);
        return (
            <ul>
                <li key="life">Awesome!</li>
            </ul>
        );
    }
}