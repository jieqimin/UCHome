toastr.options = {
    "positionClass": "toast-top-center"
};
var manskin = angular.module("skinman", ["ngTable"]);
manskin.controller("skinman", skinmanController);
skinmanController.$inject = ["NgTableParams", "$scope", "$filter", "skinmanDataService"];
function skinmanController(NgTableParams, $scope, $filter, skinmanDataService) {
    $scope.selvalue2 = "active";
    $scope.active = "active";
    $scope.showapp = true;
    $scope.parentskinpanel = true;
    $scope.initskininfo = function () {
        skinmanDataService.loaddata().then(function (list) {
            $scope.skinlist = list.data;
            var data = list.data;
            $scope.tableParams = new NgTableParams({
                page: 1,
                count: 8
            }, {
                counts: [],
                paginationMaxBlocks: 10,
                paginationMinBlocks: 1,
                total: data.length,
                getData: function ($defer, params) {
                    var filterData = params.filter() ? $filter('filter')(data, params.filter()) : data;
                    var orderedData = params.sorting() ? $filter('orderBy')(filterData, params.orderBy()) : filterData;
                    $defer.resolve($scope.skins = orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            });

        });
    };
    $scope.searchtext = "显示搜索";
    $scope.sysskin = {};
    $scope.newskinmenu = {};
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
    $scope.previewtheme = function (value) {
        $scope.childskinpanel = true;
        $scope.parentskinpanel = false;
        skinmanDataService.loadchilddata(value).then(function (list) {
            $scope.skinmenulist = list.data;
            var data = list.data;
            $scope.tableParams2 = new NgTableParams({
                page: 1,
                total: 1,
                count: 10
            }, {
                counts: [],
                paginationMaxBlocks: 10,
                paginationMinBlocks: 1,
                total: data.length,
                getData: function ($defer, params) {
                    var filterData = params.filter() ? $filter('filter')(data, params.filter()) : data;
                    var orderedData = params.sorting() ? $filter('orderBy')(filterData, params.orderBy()) : filterData;
                    $defer.resolve($scope.skins = orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
                }
            });
        });
        $scope.newskinmenu.ParentskinID = value;
        $scope.returnparent = function () {
            $scope.childskinpanel = false;
            $scope.parentskinpanel = true;
        };
    };
    $scope.curskin = {};
    $scope.removetheme = function (skinid,skinstatus) {//note: the function name is important
        delconfirm("确认要移除此项吗？", function () {
            skinmanDataService.removeskin(skinid,skinstatus).then(function (response) {
                if (response.data.Data.statuscode === "200") {
                    var index = $scope.skinlist.indexOf(skinid);
                    $scope.skinlist.splice(index, 1);
                }
                else {
                    console.log('移除失败!');
                }
            });
        }, null);
    };
    $scope.getskin = function (skin) {
        $scope.curskin = skin;
    };
    $scope.selecticon = function ($event, value) {
        var checkbox = $event.target;
        if (checkbox.checked) {
            $scope.sysskin.skinIcon = value;
        }
    };
    $scope.addskin = function (Skin) {
        Skin.PKID = Guid.NewGuid().ToString();
        Skin.SkinBackground = $("#SkinBackground").val();
        skinmanDataService.addskin(Skin).then(function (response) {
            if (response.data.Data.statuscode === "200") {
                $scope.skinlist.push(angular.copy(Skin));

                toastr["success"]("添加成功");
            }
            else {
                console.log('添加失败!');
            }
        });
    };
    $scope.modifyUser = function(newskin) {
        skinmanDataService.updateskin(newskin).then(function(response) {
            if (response.data === 'true') {
                $scope.skinlist.push(newskin);
            } else {
                console.log('更新失败');
            }
        });
    };
    $scope.initskininfo();
    $scope.checkboxes = { 'checked': false, items: {} };

    // watch for check all checkbox
    $scope.$watch('checkboxes.checked', function (value) {
        angular.forEach($scope.skins, function (item) {
            if (angular.isDefined(item.PKID)) {
                $scope.checkboxes.items[item.PKID] = value;
            }
        });
    });

    // watch for data checkboxes
    $scope.$watch('checkboxes.items', function (values) {
        if (!$scope.skins) {
            return;
        }
        var checked = 0, unchecked = 0,
            total = $scope.skins.length;
        angular.forEach($scope.skins, function (item) {
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
manskin.factory("skinmanDataService", ['skinmanHttpService', '$q', function (skinmanHttpService, $q) {
    var service = {};
    service.loaddata = function () {
        var defer = $q.defer();
        skinmanHttpService.getskin().then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.addskin = function (newskin) {
        var defer = $q.defer();
        skinmanHttpService.addskin(newskin).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.removeskin = function (skinid,skinstatus) {
        var defer = $q.defer();
        skinmanHttpService.removeskin(skinid,skinstatus).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    service.updateskin = function (newskin) {
        var defer = $q.defer();
        skinmanHttpService.updateskin(newskin).then(function (response) {
            defer.resolve(response);
        });
        return defer.promise;
    };
    return service;
}]);
//定义数据操作方法
manskin.factory("skinmanHttpService", ['$http', function ($http) {
    var service = {};
    service.getskin = function () {
        return $http.get('GetSysSkin');
    };
    service.addskin = function (newskin) {
        return $http({
            method: 'POST',
            url: 'AddSkin',
            headers: { 'Content-Type': 'application/json' },
            data: { sysskin: newskin }
        });
    };
    service.removeskin = function (skinid,skinstatus) {
        return $http.get('MoveSkin?pkid=' + skinid + '&skinstatus=' + skinstatus);
    };
    service.updateskin = function (newskin) {
        return $http({
            method: 'POST',
            url: '/api/updateskin',
            data: newskin
        });
    };
    return service;
}]);