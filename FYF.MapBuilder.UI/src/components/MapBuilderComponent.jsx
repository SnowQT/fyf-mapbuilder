import * as React from "react";
import ObjectBrowser from "./ObjectBrowser.jsx";
import { ReceiveToggle } from "../helper/NuiHelper.jsx";

class MapBuilderComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { isOpened: false }
    }

    componentDidMount() {

        const open = () => this.setState({ isOpened: true });
        const close = () => this.setState({ isOpened: false });
        ReceiveToggle(open, close);
    }

    render() {
        const { isOpened } = this.state;

        if (!isOpened) {
            return null;
        }

        return (<ObjectBrowser/>);
    }
}

export default MapBuilderComponent;