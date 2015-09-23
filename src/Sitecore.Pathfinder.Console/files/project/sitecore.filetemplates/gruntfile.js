/// <binding AfterBuild='build.project' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    grunt.initConfig({
        shell: {
            "build-project": {
                command: "scc.cmd"
            },
            "run-unittests": {
                command: "scc.cmd run-unittests"
            },
            "generate-unittests": {
                command: "scc.cmd generate-unittests"
            },
            "generate-code": {
                command: "scc.cmd generate-code"
            },
            "sync-website": {
                command: "scc.cmd sync-website"
            },
            "validate-website": {
                command: "scc.cmd validate-website"
            }
        }
    });

    grunt.registerTask("build-project", ["shell:build-project"]);
    grunt.registerTask("run-unittests", ["shell:run-unittests"]);
    grunt.registerTask("generate-unittests", ["shell:generate-unittests"]);
    grunt.registerTask("generate-code", ["shell:generate-code"]);
    grunt.registerTask("sync-website", ["shell:sync-website"]);
    grunt.registerTask("validate-website", ["shell:validate-website"]);

    grunt.loadNpmTasks("grunt-shell");
};