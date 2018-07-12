const gulp = require('gulp');
const strip = require('gulp-strip-debug');
const uglify = require('gulp-uglify');
const rename = require("gulp-rename");

gulp.task('minify', [], function () {
    var pattern = function (file) {
        file.basename = file.basename.replace('.maxi', '');
    };

    return gulp.src('*.maxi.js')
        .pipe(strip())
        .pipe(uglify())
        .pipe(rename(pattern))
        .pipe(gulp.dest(''));
});
