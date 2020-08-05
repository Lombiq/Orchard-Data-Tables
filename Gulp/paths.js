const nodeModulesBasePath = './node_modules/';
const distBasePath = './wwwroot/';

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
    lombiqAssets: [
        {
            name: 'jquery-datatables-autoinit',
            path: './Assets/Scripts/jquery-datatables-autoinit.js'
        },
        {
            name: 'lombiq-contentpicker',
            path: './Assets/Scripts/lombiq-contentpicker.js'
        },
        {
            name: 'lombiq-datatables',
            path: './Assets/Scripts/lombiq-datatables.js'
        }
    ],
    dist: {
        vendors: distBasePath + 'vendors/',
        lombiq: distBasePath + 'lombiq/'
    }
};
