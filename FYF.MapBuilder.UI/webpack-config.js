module.exports = {
    devtool: 'source-map',
    entry: "./src/main.tsx",
    mode: "production",
    output: {
        filename: "./mapbuilder-bundle.js",
        
    },
    resolve: {
        extensions: ['.Webpack.js', '.web.js', '.ts', '.js', '.jsx', '.tsx']
    },
    module: {
        rules: [
            {
                test: /\.tsx$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'ts-loader'
                }
            }
        ]
    }
}