import * as React from "react";
import { SendNuiMessage } from "../helper/NuiHelper.jsx";

import styles from "../assets/css/objectlist.css"
import global from "../assets/css/global.css"


class ObjectBrowser extends React.Component  {
    constructor(props) {
        super(props);
    }

    loadObjectJsonFile(name) {
        fetch("../assets/metadata/__entry.json")
            .then((res) => res.json())
            .then((data) => {
                console.log('data:', data);
            });
    }

    render() {
        this.loadObjectJsonFile("__entry.json");
        //Bind the "this" context to the callbacks.
        const CbObjectChanged = function () { }
        const CbObjectVariantChanged = function () { }

        return (
            <div className={styles.container}>
                <div className={styles.container_inner}>
                    <h1 className={styles.title}>Object Browser</h1>

                    <section className={styles.sub_title}>Search</section>
                    <p className={styles.description}>Enter the object name you want to search for..</p>
                    <input className={global.w100} name="search"></input>

                    <section className={styles.sub_title}>Namespaces</section>
                    <p className={styles.description}>Select the namespace you want to view the objects of. The list below will filter based on which namespace you selected.</p>

                    <select
                        name="objects" onChange={CbObjectChanged}
                        size="10" className={global.w100}
                    >
                    </select>

                    <section className={styles.sub_title}>Objects</section>
                    <p className={styles.description}>Select the object you want to view in the editor, use the namespaces list above to filter the results.</p>
                    <div>
                        <select
                            name="variants" onChange={CbObjectVariantChanged}
                            size="10" className={global.w100}
                        >
                        </select>
                    </div>
                </div>
            </div>
        );
    }
}

export default ObjectBrowser;