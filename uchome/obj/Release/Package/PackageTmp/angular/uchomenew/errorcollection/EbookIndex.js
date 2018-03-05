tempApp.controller("EbookIndex", appmanController);
appmanController.$inject = ["DTOptionsBuilder", "DTColumnDefBuilder", "$scope", "errorcontrollindexDataService"];
function appmanController(DTOptionsBuilder, DTColumnDefBuilder, $scope, errorcontrollindexDataService) {
    $scope.parentapppanel = true;
    $scope.inittable = function () {
        errorcontrollindexDataService.getData().then(function (list) {
            $scope.errorcollections = list.data;
            $scope.dtOptions = DTOptionsBuilder.newOptions().withPaginationType("full_numbers");
        });
    };
    $scope.sysapp = {};
    $scope.newappmenu = {};
    $scope.curApp = {};
    $scope.deleteData = function (pkid) {//note: the function name is important
        delconfirm("删除将无法复原，确认要删除此项吗？", function () {
            errorcontrollindexDataService.deleteData(pkid).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var eclist = $scope.errorcollections;
                    var index = -1;
                    $.each(eclist, function (i, item) {
                        if (item.PKID == pkid)
                            index = i;
                    });
                    $scope.errorcollections.splice(index, 1);
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
    $scope.changeerrorcollectionstatus = function (pkid, status) {//note: the function name is important
        delconfirm("确认执行此项操作吗？", function () {
            errorcontrollindexDataService.changeerrorcollectionstatus(pkid, status).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var eclist = $scope.errorcollections;
                    $.each(eclist, function (i, item) {
                        if (item.PKID == pkid) {
                            $scope.errorcollections[i].status = status == 1 ? 0 : 1;
                        }

                    });                   
                    toastr["success"]("操作成功");
                }
                else {
                    toastr["error"]("操作失败");
                }
            });
        }, null);
    };
    $scope.getData = function (app) {
        $scope.curApp = app;
    };
    $scope.selecticon = function ($event, value) {
        var checkbox = $event.target;
        if (checkbox.checked) {
            $scope.sysapp.AppIcon = value;
        }
    };
    $scope.addData = function (newApp, relation) {
        newApp.PKID = Guid.NewGuid().ToString();
        errorcontrollindexDataService.addapp(newApp, relation).then(function (response) {
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
        errorcontrollindexDataService.updateapp(newApp).then(function (response) {
            if (response.data === 'true') {
                $scope.errorcollection.push(newApp);
            }
            else {
                console.log('更新失败');
            }
        });
    };
    $scope.showerrorcollectiondetail = function (pkid) {
        $scope.childapppanel = true;
        $("#errorcollectiondetails").dialog({
            autoOpen: true,
            modal: true,
            width: 700
        });
        errorcontrollindexDataService.getDataByPKID(pkid).then(function (response) {
            console.log(response);
            $scope.errorcollection = response.data.ErrorCollection;
            $scope.QuestionTypeAndErrors = response.data.QuestionTypeAndErrors;
            $scope.QuestionReasonAndErrors = response.data.QuestionReasonAndErrors;
        });
    };
    $scope.inittable();
};
//定义数据操作接口
tempApp.factory("errorcontrollindexDataService", ['errorcontrollindexHttpService', '$q', function (errorcontrollindexHttpService, $q) {
    var service = {};
    service.getData = function () {
        var defer = $q.defer();
        errorcontrollindexHttpService.getData().then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.addData = function (newApp, relation) {
        var defer = $q.defer();
        errorcontrollindexHttpService.addData(newApp, relation).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.deleteData = function (pkid) {
        var defer = $q.defer();
        errorcontrollindexHttpService.deleteData(pkid).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.getDataByPKID = function (pkid) {
        var defer = $q.defer();
        errorcontrollindexHttpService.getDataByPKID(pkid).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.changeerrorcollectionstatus = function (appid, status) {
        var defer = $q.defer();
        errorcontrollindexHttpService.changeerrorcollectionstatus(appid, status).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.updateData = function (newApp) {
        var defer = $q.defer();
        errorcontrollindexHttpService.updateData(newApp).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    return service;
}]);
//定义数据操作方法
tempApp.factory("errorcontrollindexHttpService", ['$http', function ($http) {
    var service = {};
    service.getData = function () {
        return $http.get('/uchomenew/ErrorCollection/GetErrorCollections');
    };
    service.addData = function (newApp, relation) {
        return $http({
            method: 'POST',
            url: '/uchome/sysManage/AddApp',
            headers: { 'Content-Type': 'application/json' },
            data: { sysapp: newApp, relation: relation }
        });
    };
    service.getDataByPKID = function (pkid) {
        return $http.get('/uchomenew/ErrorCollection/GetErrorCollectionByPKID?pkid=' + pkid);
    };
    service.deleteData = function (pkid) {
        return $http.get('/uchomenew/ErrorCollection/DeleteErrorCollection?pkid=' + pkid);
    };
    service.changeerrorcollectionstatus = function (pkid, status) {
        return $http.get('/uchomenew/ErrorCollection/ChangeStatus?pkid=' + pkid + '&status=' + status);
    };
    service.updateData = function (newApp) {
        return $http({
            method: 'POST',
            url: '/api/updateapp',
            data: newApp
        });
    };
    return service;
}]);
