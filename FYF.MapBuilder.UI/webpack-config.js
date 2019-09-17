var path = require("path")

module.exports = {
    entry: "./src/Main.jsx",
    mode: "development",
    output: {
        path: path.resolve(__dirname, "build"),
        filename: "./mapbuilder-bundle.js",
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            }
        ]
    }
}