tempApp.filter('htmlContent', ['$sce', function ($sce) {
    return function (input) {
        return $sce.trustAsHtml(input);
    }
}]);
tempApp.controller("classActivityList", ['$scope', '$http', function ($scope, $http) {
    
    $scope.TransDate = function (date) {
        var year = date.substr(0, 4);
        var month = date.substr(4, 2);
        var day = date.substr(6, 2);
        return year + "年" + month + "月" + day + "日";
    }
    $scope.TransXNXQ = function (xnxq) {
        var xn = xnxq.substr(0, 4);
        var xq = xnxq.substr(4, 2);
        return xn + "学年第" + (xq == "1" ? "一" : "二") + "学期";
    }
    $scope.ActivityDetail = function (id) {
        $http.get("/uchome/StudentAffairs/ActivityDetail?activityId=" + id).success(function (response) {
            if (response.flag) {
                $scope.Detail = response.result;
                $("#myModal").modal("show");
            } else {
                console.info(response);
            }
        });
    }
}]);