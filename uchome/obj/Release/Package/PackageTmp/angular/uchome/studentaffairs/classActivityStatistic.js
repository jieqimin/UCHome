tempApp.controller("classActivityStatistic",["$scope","$http", function($scope, $http) {
    $scope.classList = [];
    $scope.selectedClassId = "1";
    $scope.Terms = [];
    $scope.selectedTermId = "1";
    $scope.tableData = [];
    $http.get("/uchome/studentaffairs/getBZRClasses").success(function (response) {
        if (response.flag) {
            $scope.classList = response.result;
        } else {
            console.info(response);
        }
    });
    $scope.ChangeTerms = function () {
        if ($scope.selectedClassId == "1") {
            $scope.Terms = [];
            $scope.selectedTermId = "1";
            return;
        }
        $http.get("/uchome/studentaffairs/GetClassTerms?classId="+$scope.selectedClassId).success(function(response) {
            if (response.flag) {
                $scope.Terms = response.result;
                if ($scope.Terms.length > 0) {
                    $scope.selectedTermId = $scope.Terms[0];
                }
            } else {
                console.log(response);
            }
        });
    }
    $scope.GetStaticResult = function () {
        var classId = $scope.selectedClassId;
        if (classId == "1") {
            $("#warn").text("请选择班级...").stop().fadeIn("300");
            return;
        }
        var termId = $scope.selectedTermId;
        if (termId == "1") {
            $("#warn").text("请选择学年学期...").stop().fadeIn("300");
            return;
        }
        $("#warn").stop().fadeOut("300");
        var postData = {
            classId: classId,
            termId:termId

        }
        $http.post("/uchome/studentaffairs/statiscClassActivity", postData).success(function (response) {
            if (response.flag) {
                $scope.tableData = response.result;
            } else {
                console.info(response);
            }
        });
    }
    $scope.TransXNXQ = function (xnxq) {
        var xn = xnxq.substr(0, 4);
        var xq = xnxq.substr(4, 2);
        return xn + "学年第" + (xq == "1" ? "一" : "二") + "学期";
    }
    $scope.TransDate = function (date) {
        var year = date.substr(0, 4);
        var month = date.substr(4, 2);
        var day = date.substr(6, 2);
        return year + "年" + month + "月" + day + "日";
    }
    $scope.ShowTitle = function (studentActivity, type, name) {
        $scope.showModal = {};
        $scope.showModal.list = studentActivity;
        $scope.showModal.type = type == 1 ? "社会实践" : "校园活动";
        $scope.showModal.StudentName = name;
        $("#myModal").modal("show");
    }
}])