import * as React from "react";

import styles from "../assets/css/objectlist_namespaces.css"

//@TODO(bma): Add support for folding categories.
//@TODO(bma): Don't start with everything unfolded, this gets messy real quick.
class ObjectBrowserNamespaces extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            currentRoot: ""
        };
    }

    updateRootSelection(root) {
        this.setState({ currentRoot: root });
        this.props.onRootChanged(root);
    }

    renderTreeElement(depth, name) {
        let indent = (depth * 12) + "px";
        let prefix = ""

        if (depth > 0) {
            if (depth % 2 == 0) {
                prefix = "-";
            }
            else {
                prefix = ">";
            }
        }

        return (
            <p className={styles.namespace_item} style={{ paddingLeft: indent }}>{prefix} {name}</p>
        );
    }

    renderTree(root, maxDepth) {

        if (this.props.root === null) {
            return null;
        }

        const { currentRoot } = this.state;

        if (currentRoot !== root) {
            return null;
        }

        console.log(currentRoot);

        let depth = 0;
        let namespaceStack = [];
        let treeElements = [];

        namespaceStack.push({
            depth: depth,
            root: this.props.root
        });

        //DFS traverse the namespaces from the root object.
        while (namespaceStack.length > 0) {
            const rootObj = namespaceStack.pop();
            const rootNs = rootObj.root.namespace;

            //Skip the first element, which is already defined or if it exceed the max-depth.
            if (rootObj.depth > 0 && rootObj.depth <= maxDepth) {
                treeElements.push(this.renderTreeElement(rootObj.depth, rootObj.root.name));
            }

            for (let i = 0; i < rootNs.length; i++) {
                namespaceStack.push({
                    depth: rootObj.depth + 1,
                    root: rootNs[i]
                });
            }
        }

        return treeElements;
    }

    render() {
        let rootElements = null;

        if (this.props.roots !== null) {
            rootElements = this.props.roots.map(root => {
                let { currentRoot } = this.state;
                let cssStyle = styles.namespace_item;

                let onClickCallback = (event) => {
                    this.updateRootSelection(root);

                    const element = event.target;

                    setTimeout(function (e) {
                        element.scrollIntoView({
                            behavior: "smooth",
                            block: "start",
                            inline: "nearest"
                        });
                    }, 200);
                }

                if (currentRoot === root) {
                    cssStyle = styles.namespace_item_selected;
                }

                return (
                    <div>
                        <p key={root} className={cssStyle} onClick={onClickCallback}>{root}</p>
                        <div>{ this.renderTree(root, 2) }</div>
                    </div>
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