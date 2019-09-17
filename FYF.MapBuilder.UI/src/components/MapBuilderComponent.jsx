import * as React from "react";
import ObjectListComponent from "./ObjectListComponent.jsx";
import { ReceiveNuiMessage } from "../helper/NuiHelper.jsx";

class MapBuilderComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { isOpened: true }
    }

    componentDidMount() {
        ReceiveNuiMessage("open", () => {
            this.setState({ isOpened: true });
        });

        ReceiveNuiMessage("close", () => {
            this.setState({ isOpened: false });
        });
    }

    render() {
        const { isOpened } = this.state;

        if (!isOpened) {
            return null;
        }

        return (<ObjectListComponent />);
    }
}

export default MapBuilderComponent;