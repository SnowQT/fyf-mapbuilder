/// <binding AfterBuild='clean-build-copy' />

const Gulp = require("gulp");
const GulpClean = require("gulp-clean");
const WebpackConfig = require("./webpack-config");
const WebpackStream = require("webpack-stream");

const buildPath = "./build/";
const outputPath = "../server/data/resources/fyf-mapbuilder/nui/";

const FilesToCopy = [
    "./build/mapbuilder-bundle.js",
    "./index.html",
];

const FoldersToCopy = [
    "./src/assets/**/*"
]

Gulp.task("build", () => {
    //Webpack everything together using webpack-stream.
    return WebpackStream(WebpackConfig)
        .pipe(Gulp.dest(buildPath));
});


Gulp.task("clean", () => {
    //Clean the output and build path.
    return Gulp.src([buildPath, outputPath], { read: false, allowEmpty: true })
        .pipe(GulpClean({ force: true }));
});

Gulp.task("clean-build-copy", Gulp.series(["clean", "build"], (done) => {
    //Copy over the script filed.
    Gulp.src(FilesToCopy)
        .pipe(Gulp.dest(outputPath));

    //Copy over the assets folder.
    Gulp.src(FoldersToCopy)
        .pipe(Gulp.dest(outputPath + "assets/"));

    done();
}));