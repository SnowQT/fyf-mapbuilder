const Visualizer = require("webpack-visualizer-plugin")

module.exports = {
    devtool: "source-map",
    entry: "./src/main.tsx",
    mode: "development",
    output: {
        filename: "./mapbuilder-bundle.js",
    },
    resolve: {
        extensions: [".Webpack.js", ".web.js", ".ts", ".js", ".jsx", ".tsx", ".json"]
    },
    module: {
        rules: [
            {
                test: /\.tsx$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: "ts-loader"
                }
            }
        ]
    },
    plugins: [
        new Visualizer(),
    ],
}