const gulp = require('gulp');
const watch = require('gulp-watch');
const babel = require('gulp-babel');
const paths = require('./Gulp/paths');
const jsTargets = require('../../Utilities/Lombiq.Gulp.Extensions/Tasks/js-targets');
const copyAssets = require('./Gulp/tasks/copy-assets');

gulp.task('copy:vendor-assets', () => copyAssets(paths.vendorAssets, paths.dist.vendors));
gulp.task('build:lombiq-js', () => jsTargets.compile(
    paths.lombiqAssets.base,
    paths.dist.lombiq,
    (pipeline) => pipeline.pipe(babel({ presets: ['@babel/env'] }))));

gulp.task('default', gulp.parallel('copy:vendor-assets', 'build:lombiq-js'));
gulp.task('watch:lombiq-js', () => watch(paths.lombiqAssets.base, { verbose: true }, gulp.series('build:lombiq-js')));
