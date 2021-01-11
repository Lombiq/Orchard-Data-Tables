'use strict';

const gulp = require('gulp');
const paths = require('./Gulp/paths');
const jsTargets = require('../../Utilities/Lombiq.Gulp.Extensions/Tasks/js-targets');
const copyAssets = require('./Gulp/tasks/copy-assets');

gulp.task('copy:vendor-assets', () => copyAssets(paths.vendorAssets, paths.dist.vendors));
gulp.task('copy:lombiq-assets', () => copyAssets(paths.lombiqAssets, paths.dist.lombiq));
gulp.task('default', gulp.parallel('copy:vendor-assets', 'copy:lombiq-assets'));




gulp.task('build:js-lombiq-assets',
    () => jsTargets.compile(paths.scriptsLombiq.base, './wwwroot/lombiq/'));
gulp.task('build:js-vendor-assets',
    () => jsTargets.compile(paths.scriptsVendor, paths.dist.lombiq));



gulp.task('build:jquery',
    () => jsTargets.compileOne('./Assets/Scripts/jquery-datatables-autoinit.js', './wwwroot/lombiq/jquery-datatables-autoinit'));

gulp.task('build:contentpicker',
    () => jsTargets.compileOne('./Assets/Scripts/lombiq-contentpicker.js', './wwwroot/lombiq/lombiq-contentpicker'));

gulp.task('build:datatables',
    () => jsTargets.compileOne('./Assets/Scripts/lombiq-datatables.js', './wwwroot/lombiq/lombiq-datatables'));

gulp.task('build:js-lombiq', gulp.parallel('build:jquery', 'build:contentpicker', 'build:datatables'));
