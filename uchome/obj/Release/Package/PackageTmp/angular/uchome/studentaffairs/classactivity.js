tempApp.filter('htmlContent', ['$sce', function ($sce) {
    return function (input) {
        return $sce.trustAsHtml(input);
    }
}]);
function controllerManage($scope, $http, DTOptionsBuilder, DTColumnDefBuilder) {
    $scope.classList = [];
    $scope.selectedClassId = "1";
    $scope.tableData = [];

    $scope.dtOptions = {
        "aaSorting": [[1, "desc"]
        ]
    };
    $http.get("/uchome/studentaffairs/getBZRClasses").success(function (response) {
        if (response.flag) {
            $scope.classList = response.result;
        } else {
            console.info(response);
        }
    });
    $scope.GetClassActivites = function () {
        var classId = $("#classes").val();
        if (classId == "1") {
            $("#warn").stop().fadeIn("300");
            return;
        }
        $("#warn").stop().fadeOut("300");
        var postData = {
            type: $("#type").val(),
            classId: classId
            
        }
        $http.post("/uchome/studentaffairs/getclassactivities", postData).success(function(response) {
        if (response.flag) {
            $scope.tableData = response.result;
            
        } else {
            console.info(response);
        }
    });
    }
    $scope.TransDate= function(date) {
        var year = date.substr(0, 4);
        var month = date.substr(4, 2);
        var day = date.substr(6, 2);
        return year + "年" + month + "月" + day + "日";
    }
    $scope.TransXNXQ= function(xnxq) {
        var xn = xnxq.substr(0, 4);
        var xq = xnxq.substr(4, 2);
        return xn + "学年第" + (xq == "1" ? "一" : "二") + "学期";
    }
    $scope.ActivityDetail= function(id) {
        $http.get("/uchome/StudentAffairs/ActivityDetail?activityId="+ id ).success(function (response) {
            if (response.flag) {
                $scope.Detail = response.result;
                $("#myModal").modal("show");
            } else {
                console.info(response);
            }
        });
    }
    $scope.Delete = function (id) {
        if (confirm("确认删除？")) {
            $http.post("/uchome/studentAffairs/DeleteActivity", { activityId: id }).success(function (response) {
                if (response.flag) {
                    alert("删除成功");
                    $scope.GetClassActivites();
                } else {
                    console.info(response);
                }
            });
        }
        
    }
}


tempApp.controller("classactivity", controllerManage);
tempApp.controller("classactivity2", controllerManage);