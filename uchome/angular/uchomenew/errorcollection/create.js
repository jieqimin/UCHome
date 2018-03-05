toastr.options = {
    "positionClass": "toast-bottom-center"
};
tempApp.controller("create", appmanController);
appmanController.$inject = ["$scope", "errorcontrollercreateDataService"];
function appmanController($scope, errorcontrollercreateDataService) {
    $scope.initerrorcontrollercreate = function () {
        errorcontrollercreateDataService.getData().then(function (list) {
            $scope.typelist = list.data;
        });
        errorcontrollercreateDataService.getData2().then(function (list) {
            $scope.reasonlist = list.data;
        });
    };
    $scope.diffculty = 1;
    $scope.qtype = [];
    //问题分类
    $scope.toggleselecticon = function (value) {
        var index = $scope.qtype.indexOf(value);
        if (index > -1) {
            $scope.qtype.splice(index, 1);
        } else {
            $scope.qtype.push(value);
        }
    };
    $scope.errorreason = [];
    //出错原因
    $scope.toggleselecticon2 = function (value) {
        var index = $scope.errorreason.indexOf(value);
        if (index > -1) {
            $scope.errorreason.splice(index, 1);
        } else {
            $scope.errorreason.push(value);
        }
    };
    //难易程度
    $scope.Qdiffculty = function (value) {
        $scope.diffculty = value;
    };
    //知识章节
    $scope.AddErrorQuestion = function (newerror) {
        newerror.QFiles = $("#Attachment").val();
        newerror.QAFiles = $("#Attachment2").val();
        newerror.QDifficulty = $scope.diffculty;
        newerror.QErrorTimes = 1;
        newerror.status = 0;
        newerror.QKID = $scope.selknowledge;
        newerror.QKnowledge = $scope.selknowledgePath;
        var qtype = $scope.qtype;
        var errorreason = $scope.errorreason;
        errorcontrollercreateDataService.addData(newerror, qtype, errorreason).then(function (response) {
            if (response.data.Data.statuscode === "200") {
                toastr["success"]("添加成功");
            }
            else {
                toastr["error"]("添加失败");
            }
        });
    };
    $scope.initerrorcontrollercreate();
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
tempApp.factory("errorcontrollercreateDataService", ['errorcontrollercreateService', '$q', function (errorcontrollercreateService, $q) {
    var service = {};
    service.getData = function () {
        var defer = $q.defer();
        errorcontrollercreateService.getData().then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.getData2 = function () {
        var defer = $q.defer();
        errorcontrollercreateService.getData2().then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.addData = function (newerror, qtype, errorreason) {
        var defer = $q.defer();
        errorcontrollercreateService.addData(newerror, qtype, errorreason).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    return service;
}]);
//定义数据操作方法
tempApp.factory("errorcontrollercreateService", ['$http', function ($http) {
    var service = {};
    service.getData = function () {
        return $http.get('http://jp.taedu.gov.cn:84/uchomenew/errorcollection/GetQuestionType');
    };
    service.getData2 = function () {
        return $http.get('http://jp.taedu.gov.cn:84/uchomenew/errorcollection/GetErrorReason');
    };
    service.addData = function (newerror, qtype, errorreason) {
        return $http({
            method: 'POST',
            url: 'http://jp.taedu.gov.cn:84/uchomenew/errorcollection/AddErrorQuestion',
            headers: { 'Content-Type': 'application/json' },
            data: { newerror: newerror, qtype: qtype, errorreason: errorreason }
        });
    };
    return service;
}]);
//知识章节选择控件
