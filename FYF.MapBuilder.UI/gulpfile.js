/// <binding AfterBuild='clean-build-copy' />

const gulp = require("gulp");
const gulpClean = require("gulp-clean");
const config = require("./webpack-config");
const webpackStream = require("webpack-stream");

const buildPath = "./build/";
const outputPath = "../server/data/resources/fyf-mapbuilder/nui/";

const FilesToCopy = [
    "./build/mapbuilder-bundle.js",
    "./index.html",
];

const FoldersToCopy = [
    "./src/assets/**/*"
]

gulp.task("build", () => {
    //Webpack everything together using webpack-stream.
    return webpackStream(config)
        .pipe(gulp.dest(buildPath));
});


gulp.task("clean", () => {
    //Clean the output and build path.
    return gulp.src([buildPath, outputPath], { read: false, allowEmpty: true })
        .pipe(gulpClean({ force: true }));
});

gulp.task("clean-build-copy", gulp.series(["clean", "build"], (done) => {
    //Copy over the script filed.
    gulp.src(FilesToCopy)
        .pipe(gulp.dest(outputPath));

    //Copy over the assets folder.
    gulp.src(FoldersToCopy)
        .pipe(gulp.dest(outputPath + "assets/"));

    done();
}));