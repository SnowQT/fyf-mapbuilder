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
        ReceiveNuiMessage("openCloseState", (type, data) => {
            this.setVisibility(data.state);
        });
    }

    setVisibility(state: boolean) {
        this.setState({
            isOpened: state
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
