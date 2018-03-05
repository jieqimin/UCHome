var appCenter = angular.module('appCenter', ['ui.router']);

appCenter.config(function ($stateProvider, $urlRouterProvider,$httpProvider) {
    $stateProvider
        // route to show our basic form (/form)
        .state('uchome', {
            url: '/uchome/demo',
            templateUrl: 'index',
            controller: 'sysController'
        })
        // nested states 
        // each of these sections will have their own view
        // url will be nested (/form/profile)
        .state('uchome.index', {
            url: 'index',
            templateUrl: 'index'
        })
        // url will be /form/interests
         .state('uchome.uidemo', {
             url: 'uidemo',
             templateUrl: 'uidemo'
         })
         .state('uchome.UpFileDemo', {
             url: 'datetimepickerdemo',
             templateUrl: 'datetimepickerdemo'
         })
         .state('uchome.UpFileDemo', {
             url: 'UpFileDemo',
             templateUrl: 'UpFileDemo'
         })
         .state('uchome.ValidateDemo', {
             url: 'ValidateDemo',
             templateUrl: 'ValidateDemo'
         })
         .state('uchome.DataTableDemo', {
             url: 'DataTableDemo',
             templateUrl: 'DataTableDemo'
         })
        // url will be /form/payment
       .state('uchome.formvalid', {
           url: 'formvalid',
           templateUrl: 'formvalid'
       });

    // catch all route
    // send users to the form page 
    $urlRouterProvider.otherwise('/uchome/demo/index');
    $httpProvider.defaults.withCredentials = true;
    //$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
})
if ($.isFunction(window.PageViewInit)) {
    appCenter.controller('myController', PageViewInit);
    appCenter.controller('sysController', PageViewInit);
}
else {
    appCenter.controller('myController', function ($scope) { });
    appCenter.controller('sysController', function ($scope) { });
}