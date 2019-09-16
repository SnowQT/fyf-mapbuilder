import * as React from "react";
import ObjectListComponent from "./ObjectListComponent";
import { ReceiveNuiMessage } from "./helper/NuiHelper";
    
export interface MapBuilderProps { }

interface MapBuilderState {
    isOpened: boolean;
}

class MapBuilderComponent extends React.Component<MapBuilderProps, MapBuilderState> {

    constructor(props: any) {
        super(props);

        this.state = { isOpened: false }
    }

    componentDidMount() {
        ReceiveNuiMessage("open", (data) => {
            this.setVisibility(true);
        });

        ReceiveNuiMessage("close", (data) => {
            this.setVisibility(false);
        });
    }

    setVisibility(visible: boolean) {
        console.log("Setting visibility to " + visible);
        this.setState({
            isOpened: visible
        });
    }

    render() {
        const { isOpened } = this.state;

        if (!isOpened) {
            return null;
        }

        return (
            <ObjectListComponent />
        );
    }
}

export default MapBuilderComponent;
