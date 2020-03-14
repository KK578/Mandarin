const gulp = require("gulp");

async function buildInformation() {
    const simpleGit = require("simple-git/promise");
    const fancyLog = require("fancy-log");
    const git = simpleGit();
    const fs = require("fs");
    const packageJson = require("./package.json");

    const fromVersionSha = process.env["GitVersion_VersionSourceSha"];
    const toVersionSha = process.env["GitVersion_Sha"];
    const buildNumber = process.env["GITHUB_RUN_ID"];

    if (!fromVersionSha || !toVersionSha || !buildNumber)
    {
        fancyLog.warn("Not running on GitHub CI.");
        fancyLog.info("To run locally, please set the following environment variables:");
        if (!fromVersionSha)
            fancyLog.info(" - GitVersion_VersionSourceSha = [From Version SHA]");
        if (!toVersionSha)
            fancyLog.info(" - GitVersion_Sha = [To Version SHA]")
        if (!buildNumber)
            fancyLog.info(" - GITHUB_RUN_ID = [Github Action run identifier]")
        return;
    }

    const revList = await git.raw(["log", "--pretty=format:%H|%s", `${fromVersionSha}..${toVersionSha}`]);
    const commits = revList.split("\n")
        .filter(x => x)
        .map(x => x.split("|"))
        .map(x => ({
            id: x[0],
            comment: x.splice(1).join("|")
        }));
    fancyLog.info(`Found ${commits.length} commits.`);

    const info = {
        buildEnvironment: "GitHub Actions",
        commentParser: "JIRA",
        buildNumber: buildNumber,
        buildUrl: `${packageJson.repository.url}/actions/runs/${buildNumber}`,
        vcsType: "Git",
        vcsRoot: packageJson.repository.url,
        vcsCommitNumber: toVersionSha,
        commits: commits
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
