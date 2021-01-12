'use strict';

const gulp = require('gulp');
const paths = require('./Gulp/paths');
const jsTargets = require('../../Utilities/Lombiq.Gulp.Extensions/Tasks/js-targets');
const copyAssets = require('./Gulp/tasks/copy-assets');

gulp.task('copy:vendor-assets', () => copyAssets(paths.vendorAssets, paths.dist.vendors));
gulp.task('build:jquery',
    () => jsTargets.compileOne(paths.lombiqJquery.path, paths.lombiqJquery.dest));
gulp.task('build:contentpicker',
    () => jsTargets.compileOne(paths.lombiqContentpicker.path, paths.lombiqContentpicker.dest));
gulp.task('build:datatables',
    () => jsTargets.compileOne(paths.lombiqDatatables.path, paths.lombiqDatatables.dest));
gulp.task('build:lombiq-js', gulp.parallel('build:jquery', 'build:contentpicker', 'build:datatables'));
gulp.task('default', gulp.parallel('copy:vendor-assets', 'build:lombiq-js'));
