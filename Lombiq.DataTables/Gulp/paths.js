const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';
const lombiqBasePath = './Assets/Scripts/';

module.exports = {
    vendorAssets: [
        // We are using Nightly until the Chrome issue here is resolved:
        // https://datatables.net/forums/discussion/68506/latest-google-chrome-91-0-4472-77-breaks-fixed-columns-with-complex-headings
        // {
        //     name: 'datatables.net',
        //     path: nodeModulesBasePath + 'datatables.net/js/*',
        // },
        {
            name: 'datatables.net-buttons',
            path: nodeModulesBasePath + 'datatables.net-buttons/js/*',
        },
        // Same as above.
        // {
        //     name: 'datatables.net-bs4-js',
        //     path: nodeModulesBasePath + 'datatables.net-bs4/js/*',
        // },
        // {
        //     name: 'datatables.net-bs4-css',
        //     path: nodeModulesBasePath + 'datatables.net-bs4/css/*',
        // },
        {
            name: 'datatables.net-bs4-js',
            path: nodeModulesBasePath + 'datatables.net-buttons-bs4/js/*',
        },
        {
            name: 'datatables.net-bs4-css',
            path: nodeModulesBasePath + 'datatables.net-buttons-bs4/css/*',
        },
        {
            name: 'urijs',
            path: nodeModulesBasePath + 'urijs/src/**',
        },
    ],
    lombiqAssets: {
        base: lombiqBasePath,
        all: lombiqBasePath + '**/*.js',
    },
    dist: {
        vendors: distBasePath + 'vendors/',
        lombiq: distBasePath + 'lombiq/',
    },
};
