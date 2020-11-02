const gulp = require('gulp');
const terser = require('gulp-terser');
const strip = require('gulp-strip-debug');
const rename = require("gulp-rename");

function minify() {
    var pattern = function (file) {
        file.basename = file.basename.replace('.maxi', '');
    };
    
    return gulp.src('treeview.maxi.js')
        .pipe(strip())
        .pipe(terser().on('error', function (uglify) {
            console.log("An error occurred in minification (gulp-terser) " + uglify.toString());
            console.error(uglify.message);
            this.emit('end');
        }))
        .pipe(rename(pattern))
        .pipe(gulp.dest('.'));
}
gulp.task(minify);
