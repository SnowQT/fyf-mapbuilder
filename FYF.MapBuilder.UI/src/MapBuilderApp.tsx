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
        ReceiveNuiMessage("openCloseState", (data) => {
            this.setVisibility();
        });
    }

    setVisibility() {
        const { isOpened } = this.state;
        let newState = !isOpened;

        this.setState({
            isOpened: newState
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
