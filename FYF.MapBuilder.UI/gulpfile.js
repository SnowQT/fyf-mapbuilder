/// <binding AfterBuild='deploy' />

const gulp = require("gulp");
const wp = require("webpack")
const wpconfig = require("./webpack-config");
var bs = require("browser-sync").create();

const outputPath = "../server/data/resources/fyf-mapbuilder/nui/";

const FilesToCopy = [
    "./build/mapbuilder-bundle.js",
    "./index.html",
];

const FoldersToCopy = [
    "./src/assets/**/*"
]

//Builds and deploys the code to the FiveM server.
gulp.task("deploy", (done) => {
    //Let webpack build...
    return wp(wpconfig).run(() => {
        //Copy over the script files.
        gulp.src(FilesToCopy)
            .pipe(gulp.dest(outputPath));

        //Copy over the assets folder.
        gulp.src(FoldersToCopy)
            .pipe(gulp.dest(outputPath + "assets/"));

        done();
    });
});

//Sets up a live environment for development.
gulp.task("live", () => {
    bs.init({
        server: {
            baseDir: "./build",
            index: "index.html",
        }
    });

    //Watch for changes to the index.html.
    gulp.watch("index.html").on("change", () => {
        gulp.src("index.html")
            .pipe(gulp.dest("build"));

        bs.reload();
    });

    //Watch for any changes to jsx, requires a recompile.
    gulp.watch("src/**/*.jsx").on("change", () => {
        wp(wpconfig).run(() => {
            bs.reload();
        });
    });

    //Watch for changes in the style sheets.
    //@TODO: Possibility to use bs.stream, might overcome the building overhead of webpack.
    gulp.watch("src/assets/**/*.css").on("change", () => {
        wp(wpconfig).run(() => {
            bs.reload();
        });
    });
});