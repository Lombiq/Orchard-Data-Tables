const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';
const lombiqBasePath = './Assets/Scripts/';

module.exports = {
    vendorAssets: [
        {
            name: 'datatables.net',
            path: nodeModulesBasePath + 'datatables.net/js/*',
        },
        {
            name: 'datatables.net-buttons',
            path: nodeModulesBasePath + 'datatables.net-buttons/js/*',
        },
        {
            name: 'datatables.net-bs4-js',
            path: nodeModulesBasePath + 'datatables.net-bs4/js/*',
        },
        {
            name: 'datatables.net-bs4-css',
            path: nodeModulesBasePath + 'datatables.net-bs4/css/*',
        },
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
