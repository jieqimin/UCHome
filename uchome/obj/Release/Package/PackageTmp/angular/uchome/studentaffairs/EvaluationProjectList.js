tempApp.controller("EvaluationProjectList", ["$scope", "$http", function ($scope, $http) {
    $scope.GetAllProjects= function() {
        $http.get("/uchome/studentAffairs/GetAllProjects").success(function (response) {
            if (response.flag) {
                $scope.List = response.result;
            } else {
                console.info(response);
            }
        });
    }
    $scope.GetAllProjects();
    $scope.TransTime = function (time) {
        time = parseInt(time.replace(/\D/igm, ""));
        time = new Date(time);
        time = time.getFullYear() + "年" + (time.getMonth() + 1) + "月" + time.getDate() + "日 " + time.getHours() + ":" + (time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes());
        return time;
    }
    $scope.ProjectDelete= function(id) {
        $http.get("/uchome/studentaffairs/EvaluationProjectDelete?projectId=" + id).success(function(response) {
            if (response.flag) {
               
                    alert("删除成功");
                    $scope.GetAllProjects();
               
            }
        });
    }
    $scope.ProjectDetail = function (projectId) {
        $http.get("/uchome/studentAffairs/GetEvaluationProjectDetail?projectId=" + projectId).success(function (response) {
            response = eval("(" + response + ")");
            if (response.flag) {
                $scope.ProjectInfo = response.result;
                console.info(response);
                $("#myModal").modal("show");
            } else {
                console.info(response);
            }
        });
    }
    $scope.transInfo = function (type, value) {
        switch (type) {
            case "type":
                switch (value) {
                    case "student":
                        return "学生";
                    case "class":
                        return "班级";
                    case "teacher":
                        return "教师";
                }
            case "circle":
                switch (value) {
                    case "day":
                        return "天";
                    case "week":
                        return "周";
                    case "month":
                        return "月";
                }
            default:
                return "";
        }
    }
}])