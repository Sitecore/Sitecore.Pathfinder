/// <binding AfterBuild='build' />
var gulp = require("gulp");
var shell = require("gulp-shell");

gulp.task("build",
    shell.task([
        "scc.cmd"
    ])
);