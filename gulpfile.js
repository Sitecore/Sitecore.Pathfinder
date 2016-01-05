var gulp = require("gulp");
var del = require("del");
var msbuild = require("gulp-msbuild");
var runSequence = require("run-sequence");
var spawn = require("child_process").spawn;
var zip = require("gulp-zip");

gulp.task("clean-dist-directory", function () {
    return del("./build/dist");
});

gulp.task("build-dist-directory", ["clean-dist-directory"], function () {
    return gulp.src(["./bin/files/**/*", "./bin/licenses/**/*", "./bin/*.dll", "./bin/scc.exe", "./bin/scc.exe.config", "./bin/scconfig.json"], { base: "./bin/" }).
        pipe(gulp.dest("./build/dist"));
});

gulp.task("clean-npm-directory", function () {
    return del("./build/npm");
});

gulp.task("copy-npm-files", ["clean-npm-directory"], function () {
    return gulp.src(["./bin/npm/package.json", "./bin/npm/README.md"]).
        pipe(gulp.dest("./build/npm"));
});

gulp.task("copy-npm-directory", ["clean-npm-directory", "copy-npm-files"], function () {
    return gulp.src(["./build/dist/**/*"]).
        pipe(gulp.dest("./build/npm/"));
});

gulp.task("build-npm-package", ["copy-npm-directory"], function () {
    return spawn("npm.cmd", ["pack"], { stdio: "inherit", "cwd": "./build/npm/" });
});

gulp.task("publish-npm-package", ["copy-npm-directory"], function () {
    return spawn("npm.cmd", ["publish"], { stdio: "inherit", "cwd": "./build/npm/" });
});

gulp.task("build-project", function () {
    return gulp.src("./Sitecore.Pathfinder.sln").pipe(msbuild({
        targets: ["Clean", "Build"],
        configuration: "Debug",
        logCommand: false,
        verbosity: "minimal",
        maxcpucount: 0,
        toolsVersion: 14.0
    }));
});

gulp.task("clean-zip-file", function () {
    return del("./build/Sitecore.Pathfinder.zip");
});

gulp.task("build-zip-file", ["clean-zip-file"], function () {
    return gulp.src(["build/dist/**/*"]).
        pipe(zip("Sitecore.Pathfinder.zip")).
        pipe(gulp.dest("build"));
});

gulp.task("default", function () {
    runSequence("build-project", "build-dist-directory", "build-zip-file", "publish-npm-package");
});
