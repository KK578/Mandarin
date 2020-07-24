const gulp = require("gulp");

async function buildInformation() {
    const simpleGit = require("simple-git");
    const fancyLog = require("fancy-log");
    const git = simpleGit();
    const fs = require("fs");
    const packageJson = require("./package.json");

    const toVersionSha = process.env["GITHUB_SHA"];
    const buildNumber = process.env["GITHUB_RUN_ID"];

    if (!toVersionSha || !buildNumber)
    {
        fancyLog.warn("Not running on GitHub CI.");
        fancyLog.info("To run locally, please set the following environment variables:");
        if (!toVersionSha)
            fancyLog.info(" - GITHUB_SHA = [Current Tag]")
        if (!buildNumber)
            fancyLog.info(" - GITHUB_RUN_ID = [Github Action run identifier]")
        return;
    }

    const rawFromVersionSha = await git.raw(["describe", "--abbrev=0", "--tags", `${toVersionSha}^`]);
    const fromVersionSha = rawFromVersionSha.split("\n")[0];

    const commits = await git.log({
        from: fromVersionSha,
        to: toVersionSha,
        format: {
            id: "%H",
            comment: "%s"
        }
    }, x => x);

    fancyLog.info(`Found ${commits.all.length} commits.`);

    const info = {
        buildEnvironment: "GitHub Actions",
        commentParser: "JIRA",
        buildNumber: buildNumber,
        buildUrl: `${packageJson.repository.url}/actions/runs/${buildNumber}`,
        vcsType: "Git",
        vcsRoot: packageJson.repository.url,
        vcsCommitNumber: toVersionSha,
        commits: commits.all
    };
    fancyLog.info(info);

    fs.writeFileSync("BuildInformation.json", JSON.stringify(info));
}

function css() {
    const debug = require("gulp-debug");
    const postcss = require("gulp-postcss");
    const cleanCss = require("gulp-clean-css");
    const sourceMaps = require("gulp-sourcemaps");
    const rename = require("gulp-rename");

    return gulp.src("./Styles/*.pcss")
        .pipe(debug({
            title: "src:"
        }))
        .pipe(postcss([
            require("precss"),
            require("tailwindcss"),
            require("autoprefixer")
        ]))
        .pipe(rename({extname: ".css"}))
        .pipe(sourceMaps.init())
        .pipe(cleanCss())
        .pipe(sourceMaps.write("./"))
        .pipe(gulp.dest("./wwwroot/css/"))
        .pipe(debug({
            title: "output:"
        }))
}

gulp.task(buildInformation);
gulp.task(css);
gulp.task("default", css);
