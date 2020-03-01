const gulp = require("gulp");

function css() {
    const postcss = require("gulp-postcss");
    const cleanCss = require("gulp-clean-css");
    const sourceMaps = require("gulp-sourcemaps");
    const rename = require("gulp-rename");

    return gulp.src("./Styles/*.pcss")
        .pipe(postcss([
            require("precss"),
            require("tailwindcss"),
            require("autoprefixer")
        ]))
        .pipe(rename({extname: ".css"}))
        .pipe(sourceMaps.init())
        .pipe(cleanCss())
        .pipe(sourceMaps.write("./"))
        .pipe(gulp.dest("./wwwroot/css/"));
}

gulp.task(css);
gulp.task("default", css);
