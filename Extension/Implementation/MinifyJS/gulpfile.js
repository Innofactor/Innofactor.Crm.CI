const gulp = require('gulp');
const strip = require('gulp-strip-debug');
const uglify = require('gulp-uglify');
const rename = require("gulp-rename");

function minify() {
    var pattern = function (file) {
        file.basename = file.basename.replace('.maxi', '');
    };
    
    return gulp.src('*.maxi.js')
        .pipe(strip())
        .pipe(uglify().on('error', function (uglify) {
            console.log("An error occurred in minification (uglify) " + uglify.toString());
            console.error(uglify.message);
            this.emit('end');
        }))
        .pipe(rename(pattern))
        .pipe(gulp.dest('.'));
}
gulp.task(minify);
