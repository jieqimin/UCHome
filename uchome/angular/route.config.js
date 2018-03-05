"use strict";
tempApp.config(["$stateProvider", "$urlRouterProvider", "$locationProvider", routeFn]);
function routeFn($stateProvider, $urlRouterProvider, $locationProvider) {
    //$urlRouterProvider.when("", "/create");
    $stateProvider
        // route to show our basic form (/form)
        .state('uchome', {
            url: '/uchome'
        }).state('building', {
            url: '/building',
            templateUrl: "/uchome/publicshare/building"
        });
    //通过AppID获取AppMenu
    var d = window.menulists;
    //去除多余的js加载项

    angular.forEach(d.data, function (item, index) {
        $stateProvider.state(item.controllname, {
            url: '/' + item.controllname,
            templateUrl: item.menuhref,
            controller: item.controllname,
            cache:false,
            resolve: {
                deps: function ($ocLazyLoad) {
                    var controllerurl = "/uchome/angular" + item.datapath + ".js";
                    clearjs(d.data);
                    if (isExistFile(controllerurl))
                        return $ocLazyLoad.load(controllerurl);
                }
            }
        });
    });
    $urlRouterProvider.when("","/"+d.data[0].controllname);


    //for (var i = 0; i < window.menucount; i++) {
    //    $stateProvider.state('menu' + i, {
    //        url: '/menu' + i + '/{src}/{datapath}/{controllname}',
    //        templateUrl: getTemplateUrl,
    //        controllerProvider: controllername,
    //        resolve: {
    //            deps: function ($ocLazyLoad, $stateParams) {
    //                var controllerurl = "/uchome/angular" + $stateParams.datapath + ".js";
    //                if (isExistFile(controllerurl))
    //                    return $ocLazyLoad.load(controllerurl);
    //            }
    //        }
    //    });
    //};
    function clearjs(objects) {
        $.each(objects, function (index, item) {
            console.log(item);
            var controllerurl = "/uchome/angular" + item.datapath + ".js";
            $("script[src='" + controllerurl + "']").remove();
        });
    }
    function isExistFile(filepath) {
        var flag = false;
        var xmlhttp;
        if (window.XMLHttpRequest) {
            xmlhttp = new XMLHttpRequest();
        }
        else if (window.ActiveXObject) {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.open("GET",filepath, false);
        xmlhttp.send();
        if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
            flag = true;
        }
        return flag;
    }
    //function controllername($stateParams) {
    //    var controllerurl = "/uchome/angular" + $stateParams.datapath + ".js";
    //    if (isExistFile(controllerurl))
    //        return $stateParams.controllname;
    //}
    //function getTemplateUrl($params) {
    //    return $params.src;
    //}
};
tempApp.run(['$rootScope', '$state', '$templateCache', function ($rootScope, $state, $templateCache) {
    var obj = $state;
    $rootScope.$on('$stateChangeStart', function () {
        $("#id-app-content").html("<div class='loading'>正在加载应用，请稍候…</div>");
    });
    $rootScope.$on('$stateNotFound', function (event) {
        $("#id-app-content").load("/uchomenew/publicshare/building");
        event.preventDefault();
    });
    $rootScope.$on('$viewContentLoaded', function() {
        $templateCache.removeAll();
    });
}]);
tempApp.controller("uchomecontroller", function PageViewInit($scope) {
    $scope.name = "Testt";
    $scope.submitForm = function (isValid) {
        if (!isValid) {
            alert('验证失败');
        }
        else {
            alert('fail');
        }
    };
});





