var gulp = require("gulp");
var clean = require("gulp-clean");
var msbuild = require("gulp-msbuild");
var zip = require("gulp-zip");

gulp.task("build-dist-directory", function () {
    console.log("Building distribution directory...");

    gulp.src("build/dist", { read: false }).pipe(clean());

    gulp.src(["bin/files/**/*"], { read: false }).pipe(gulp.dest("build/dist/files"));
    gulp.src(["bin/licenses/**/*"], { read: false }).pipe(gulp.dest("build/dist/licenses"));
    gulp.src(["bin/*.dll"], { read: false }).pipe(gulp.dest("build/dist"));
    gulp.src(["bin/scc.exe"], { read: false }).pipe(gulp.dest("build/dist"));
    gulp.src(["bin/scc.exe.config"], { read: false }).pipe(gulp.dest("build/dist"));
    gulp.src(["bin/scconfig.json"], { read: false }).pipe(gulp.dest("build/dist"));
});

gulp.task("build-project", function () {
    console.log("Building project...");

    return gulp.src("./Sitecore.Pathfinder.sln").pipe(msbuild({
        targets: ["Clean", "Build"],
        configuration: "Debug",
        logCommand: false,
        verbosity: "minimal",
        maxcpucount: 0,
        toolsVersion: 14.0
    }));
});

gulp.task("build-zip-file", function() {
    console.log("Building zip file...");

    gulp.src("build/Sitecore.Pathfinder.zip", { read: false }).pipe(clean());

    return gulp.src(["build/dist/**/*"]).
        pipe(zip("Sitecore.Pathfinder.zip")).
        pipe(gulp.dest("build"));
});

gulp.task("clean-dist-directory", function () {
    console.log("Cleaning distribution directory...");

    gulp.src("build/dist", { read: false }).pipe(clean());
});


gulp.task("default", ["build-project", "build-dist-directory", "build-zip-file"]);