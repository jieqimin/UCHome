toastr.options = {
    "positionClass": "toast-top-center"
};
var manapp = angular.module('roleman', ['datatables']);
manapp.controller("roleman", appmanController);
appmanController.$inject = ["DTOptionsBuilder", "DTColumnDefBuilder", "$scope", "appmanDataService"];
function appmanController(DTOptionsBuilder, DTColumnDefBuilder, $scope, appmanDataService) {
    $scope.selvalue1 = "active";
    $scope.active = "active";
    $scope.showapp = true;
    $scope.parentapppanel = true;
    $scope.initappinfo = function () {
        appmanDataService.loaddata().then(function (list) {
            $scope.applist = list.data;
            $scope.dtOptions = DTOptionsBuilder.newOptions().withPaginationType("full_numbers");
        });
    };
    $scope.searchtext = "显示搜索";
    $scope.sysapp = {};
    $scope.newappmenu = {};
    $scope.searchShoworHide = function () {
        console.log($scope.showCheckbox);
        if ($scope.showCheckbox) {
            $scope.showCheckbox = false;
            $scope.searchtext = "显示搜索";
        } else {
            $scope.showCheckbox = true;
            $scope.searchtext = "隐藏搜索";
        }
    };
    $scope.editchildapp = function (value) {
        $scope.childapppanel = true;
        $scope.parentapppanel = false;
        appmanDataService.loadchilddata(value).then(function (list) {
            $scope.appmenulist = list.data;
        });

        $scope.newappmenu.ParentAppID = value;
        $scope.returnparent = function () {
            $scope.childapppanel = false;
            $scope.parentapppanel = true;
        };
    };
    $scope.curApp = {};
    $scope.deleteapp = function (appid) {//note: the function name is important
        delconfirm("删除将无法复原，确认要删除此项吗？", function () {
            appmanDataService.deleteapp(appid).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var index = $scope.applist.indexOf(appid);
                    $scope.applist.splice(index, 1);
                    toastr["error"]("删除成功");
                }
                else if (response.data.Data.statuscode === "300") {
                    toastr["warning"](response.data.Data.msg);
                }
                else {
                    toastr["error"]("删除失败");
                }
            });
        }, null);
    };
    $scope.removeapp = function (appid, appstatus) {//note: the function name is important
        delconfirm("确认执行此项操作吗？", function () {
            appmanDataService.removeapp(appid, appstatus).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var applist = $scope.applist;
                    var index = -1;
                    $.each(applist, function (i, item) {
                        if (item.PKID == appid)
                            index = i;
                    });
                    $scope.applist.splice(index, 1, angular.copy(response.data.Data.newapp));
                    toastr["success"]("操作成功");
                }
                else {
                    toastr["error"]("操作失败");
                }
            });
        }, null);
    };
    $scope.getApp = function (app) {
        $scope.curApp = app;
    };
    $scope.selecticon = function ($event, value) {
        var checkbox = $event.target;
        if (checkbox.checked) {
            $scope.sysapp.AppIcon = value;
        }
    };
    $scope.addApp = function (newApp, relation) {
        newApp.PKID = Guid.NewGuid().ToString();
        appmanDataService.addapp(newApp, relation).then(function (response) {
            if (response.data.Data.statuscode === "200") {
                $scope.applist.push(newApp);
                toastr["success"]("添加成功");
            }
            else {
                toastr["error"]("添加失败");
            }
        });
    };
    $scope.modifyUser = function (newApp) {
        appmanDataService.updateapp(newApp).then(function (response) {
            if (response.data === 'true') {
                $scope.applist.push(newApp);
            }
            else {
                console.log('更新失败');
            }
        });
    };
    $scope.addchildApp = function (newApp) {
        var menuid = Guid.NewGuid().ToString();
        newApp.PKID = menuid;
        appmanDataService.addchildapp(newApp).then(function (response) {
            if (response.data.Data.statuscode === "200") {
                $scope.appmenulist.push(angular.copy(newApp));
                toastr["success"]("添加成功");
            }
            else {
                toastr["error"]("添加失败");
            }
        });
    };
    $scope.deletechildapp = function (pkid) {
        delconfirm("确认要删除此项吗？", function () {
            appmanDataService.deletechildapp(pkid).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var menulist = $scope.appmenulist;
                    var index = -1;
                    $.each(menulist, function (i, item) {
                        if (item.PKID == pkid)
                            index = i;
                    });
                    $scope.appmenulist.splice(index, 1);
                    toastr["success"]("删除成功");
                }
                else {
                    toastr["error"]("删除失败");
                }
            });
        }, null);
    };
    $scope.initappinfo();
    $scope.checkboxes = { 'checked': false, items: {} };

    // watch for check all checkbox
    $scope.$watch('checkboxes.checked', function (value) {
        angular.forEach($scope.Apps, function (item) {
            if (angular.isDefined(item.PKID)) {
                $scope.checkboxes.items[item.PKID] = value;
            }
        });
    });

    // watch for data checkboxes
    $scope.$watch('checkboxes.items', function (values) {
        if (!$scope.Apps) {
            return;
        }
        var checked = 0, unchecked = 0,
            total = $scope.Apps.length;
        angular.forEach($scope.Apps, function (item) {
            checked += ($scope.checkboxes.items[item.PKID]) || 0;
            unchecked += (!$scope.checkboxes.items[item.PKID]) || 0;
        });
        if ((unchecked == 0) || (checked == 0)) {
            $scope.checkboxes.checked = (checked == total);
        }
        // grayed checkbox
        angular.element(document.getElementById("select_all")).prop("indeterminate", (checked != 0 && unchecked != 0));
    }, true);
};
//定义数据操作接口
manapp.factory("appmanDataService", ['appmanHttpService', '$q', function (appmanHttpService, $q) {
    var service = {};
    service.loaddata = function () {
        var defer = $q.defer();
        appmanHttpService.getApp().then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.addapp = function (newApp, relation) {
        var defer = $q.defer();
        appmanHttpService.addApp(newApp, relation).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.deleteapp = function (appid) {
        var defer = $q.defer();
        appmanHttpService.deleteApp(appid).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.removeapp = function (appid, appstatus) {
        var defer = $q.defer();
        appmanHttpService.removeApp(appid, appstatus).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.updateapp = function (newApp) {
        var defer = $q.defer();
        appmanHttpService.updateApp(newApp).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.loadchilddata = function (parentappid) {
        var defer = $q.defer();
        appmanHttpService.getchildApp(parentappid).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.addchildapp = function (newChildApp) {
        var defer = $q.defer();
        appmanHttpService.addchildApp(newChildApp).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.deletechildapp = function (parentappid) {
        var defer = $q.defer();
        appmanHttpService.deletechildApp(parentappid).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.updatechildapp = function (newChildApp) {
        var defer = $q.defer();
        appmanHttpService.updatechildApp(newChildApp).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    return service;
}]);
//定义数据操作方法
manapp.factory("appmanHttpService", ['$http', function ($http) {
    var service = {};
    service.getApp = function () {
        return $http.get('/uchome/sysManage/GetSysApp');
    };
    service.addApp = function (newApp, relation) {
        return $http({
            method: 'POST',
            url: '/uchome/sysManage/AddApp',
            headers: { 'Content-Type': 'application/json' },
            data: { sysapp: newApp, relation: relation }
        });
    };
    service.deleteApp = function (appid) {
        return $http.get('/uchome/sysManage/DeleteApp?pkid=' + appid);
    };
    service.removeApp = function (appid, appstatus) {
        return $http.get('/uchome/sysManage/MoveApp?pkid=' + appid + '&appstatus=' + appstatus);
    };
    service.updateApp = function (newApp) {
        return $http({
            method: 'POST',
            url: '/api/updateapp',
            data: newApp
        });
    };
    service.getchildApp = function (parentAppid) {
        return $http.get('/uchome/sysManage/GetAppmenus?parentAppid=' + parentAppid);
    };
    service.addchildApp = function (newChildApp) {
        return $http({
            method: 'POST',
            url: '/uchome/sysManage/AddAppmenu',
            headers: { 'Content-Type': 'application/json' },
            data: newChildApp
        });
    };
    service.deletechildApp = function (parentAppid) {
        return $http.get('/uchome/sysManage/DeleteAppmenu?pkid=' + parentAppid);
    };
    service.updatechildApp = function (newChildApp) {
        return $http({
            method: 'POST',
            url: '/api/updateapp',
            data: newChildApp
        });
    };
    return service;
}]);