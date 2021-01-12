const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';
const lombiqBasePath = './Assets/Scripts/';
const lombiqDistPath = distBasePath + 'lombiq/';

module.exports = {
    vendorAssets: [
        {
            name: 'datatables.net',
            path: nodeModulesBasePath + 'datatables.net/js/*'
        },
        {
            name: 'datatables.net-buttons',
            path: nodeModulesBasePath + 'datatables.net-buttons/js/*'
        },
        {
            name: 'datatables.net-bs4-js',
            path: nodeModulesBasePath + 'datatables.net-bs4/js/*'
        },
        {
            name: 'datatables.net-bs4-css',
            path: nodeModulesBasePath + 'datatables.net-bs4/css/*'
        },
        {
            name: 'datatables.net-bs4-js',
            path: nodeModulesBasePath + 'datatables.net-buttons-bs4/js/*'
        },
        {
            name: 'datatables.net-bs4-css',
            path: nodeModulesBasePath + 'datatables.net-buttons-bs4/css/*'
        },
        {
            name: 'urijs',
            path: nodeModulesBasePath + 'urijs/src/**'
        }
    ],
    lombiqJquery: {
        name: 'jquery-datatables-autoinit',
        // path: './Assets/Scripts/jquery-datatables-autoinit.js'
        path: lombiqBasePath + 'jquery-datatables-autoinit.js',
        dest: lombiqDistPath + 'jquery-datatables-autoinit'
    },
    lombiqContentpicker: {
        name: 'lombiq-contentpicker',
        // path: './Assets/Scripts/lombiq-contentpicker.js'
        path: lombiqBasePath + 'lombiq-contentpicker.js',
        dest: lombiqDistPath + 'lombiq-contentpicker'
    },
    lombiqDatatables: {
        name: 'lombiq-datatables',
        // path: './Assets/Scripts/lombiq-datatables.js'
        path: lombiqBasePath + 'lombiq-datatables.js',
        dest: lombiqDistPath + 'lombiq-datatables'
    },
    dist: {
        vendors: distBasePath + 'vendors/',
        lombiq: distBasePath + 'lombiq/'
    }
};
