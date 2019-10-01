import * as React from "react";

import styles from "../assets/css/objectlist_namespaces.css"

class ObjectBrowserNamespaces extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            currentRoot: ""
        };
    }

    updateRootSelection(root) {
        console.log("Changed root to: " + root);

        this.setState({ currentRoot: root });
        this.props.onRootChanged(root);
    }

    render() {
        let rootElements = null;

        if (this.props.roots !== null) {
            rootElements = this.props.roots.map(root => {

                let { currentRoot } = this.state;
                let cssStyle = styles.namespace_item;

                let onClickCallback = (event) => {
                    this.updateRootSelection(root);
                }

                if (currentRoot === root) {
                    cssStyle = styles.namespace_item_selected;
                }

                return (
                    <p key={root} className={cssStyle} onClick={onClickCallback}>{root}</p>
                );
            });
        }
        
        return (
            <div className={styles.namespace_container}>
               { rootElements }
            </div>
        );
    }
}

export default ObjectBrowserNamespaces;