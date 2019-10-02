import * as React from "react";
import { SendNuiMessage } from "../helper/NuiHelper.jsx";
import ObjectBrowserNamespaces from "./ObjectBrowserNamespaces.jsx";

import styles from "../assets/css/objectlist.css"
import global from "../assets/css/global.css"


class ObjectBrowser extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            roots: null,
            selectedRoot: null
        };

        this.getRoots();
    }

    async getRoots() {
        const file = await this.loadRootFile("__entry");

        if (file.Roots === undefined) {
            console.warning("Failed to load __entry.json!");
            return;
        }

        this.setState({ roots: file.Roots });
    }

    async loadRootFile(name) {
        try {
            const path = `../assets/metadata/${name}.json`;
            const response = await fetch(path);
            const json = response.json();

            return json;
        }
        catch (error) {
            console.error(`Could not load ${name}.json from object definitions`)
            return "";
        }
    }

    async updateRootSelection(root) {
        const loadedRoot = await this.loadRootFile(root);
        this.setState({ selectedRoot: loadedRoot });
    }

    render() {
        const { roots, selectedRoot } = this.state;
        const updateRootSelectionCb = root => this.updateRootSelection(root);

        let objectElements = null;

        if (selectedRoot !== null) {
            console.wr
            objectElements = selectedRoot.objects.map(obj => {
                return (
                    <option key={obj}>{obj}</option>
                );
            }); 
        }

        return (
            <div className={styles.container}>
                <div className={styles.container_inner}>
                    <h2 className={styles.title}>Object Browser</h2>

                    <div>
                        <section className={styles.sub_title}>Search</section>
                        <p className={styles.description}>Enter the object name you want to search for. Searching will begin after submitting an underscore. (i.e. "stt_")</p>
                        <input autoComplete="off" className={global.w100} name="search"></input>
                    </div>

                    <div>
                        <section className={styles.sub_title}>Namespaces</section>
                        <p className={styles.description}>Select the namespace you want to view the objects of. The list below will filter based on which namespace you selected.</p>
                        <ObjectBrowserNamespaces onRootChanged={updateRootSelectionCb} onFilterObjects={roots} roots={roots} root={selectedRoot} />
                    </div>

                    <div>
                        <section className={styles.sub_title}>Objects</section>
                        <p className={styles.description}>Select the object you want to view in the editor, use the namespaces list above to filter the results.</p>
                        <select name="objects" size="10" className={global.w100} >
                            { objectElements }
                        </select>
                    </div>
                </div>
            </div>
        );
    }
}

export default ObjectBrowser;