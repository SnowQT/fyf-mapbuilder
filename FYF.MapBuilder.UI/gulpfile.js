/// <binding AfterBuild='clean-copy-build' />

const Gulp = require("gulp");
const GulpClean = require("gulp-clean");
const Webpack = require("webpack")
const WebpackConfig = require("./webpack-config");
const WebpackStream = require("webpack-stream");

const buildPath = "./build/";
const outputPath = "../server/data/resources/fyf-mapbuilder/nui/";

const ItemsToCopyToOutput = [
    "./build/mapbuilder-bundle.js",
    "./index.html",
    "./assets/*"
];

Gulp.task("build", () => {
    return WebpackStream(WebpackConfig)
        .pipe(Gulp.dest(buildPath));
});

Gulp.task("clean", () => {
    return Gulp.src([buildPath, outputPath], { read: false, allowEmpty: true })
        .pipe(GulpClean({ force: true }));
});

Gulp.task("clean-copy-build", Gulp.series(["clean", "build"], (done) => {
    Gulp.src(ItemsToCopyToOutput)
        .pipe(Gulp.dest(outputPath));
    done();
}));