const gulp = require('gulp');
const terser = require('gulp-terser');
const strip = require('gulp-strip-debug');
const rename = require('gulp-rename');
const pluginError = require('plugin-error');

const pattern = function (file) {
  file.basename = file.basename.replace('.maxi', '');
};

exports.minify = async function () {
  return gulp
    .src('*.maxi.js')
    .pipe(strip())
    .pipe(
      terser().on('error', function (uglify) {
        const errorMessage =
          'An error occurred in minification (gulp-terser) ' +
          uglify.toString();
        console.log(errorMessage);
        console.error(uglify.message);
        var err = new pluginError('minifyJS', errorMessage, {
          showStack: true,
        });
        this.emit('end');
      })
    )
    .pipe(rename(pattern))
    .pipe(gulp.dest('.'));
};
