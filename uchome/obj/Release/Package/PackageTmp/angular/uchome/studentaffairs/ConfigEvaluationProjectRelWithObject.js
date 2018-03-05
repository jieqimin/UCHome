tempApp.controller("ConfigEvaluationProjectRelWithObject", ["$scope", "$http", function ($scope, $http) {
    $scope.rows = [];
    
    $scope.ProjectList = [];
    $scope.evalutatedType = "teacher";
    $http.get("/uchome/StudentAffairs/GetAllProjects").success(function(response) {
        if (response.flag) {
            $scope.ProjectList = response.result;
        } else {
            console.info(response);
        }
    });
    $scope.ChangeColomn= function() {
        $http.get("/uchome/studentaffairs/getProjectColomns?projectId=" + $scope.Project.Id).success(function (response) {
            if (response.flag) {
                $scope.colomns=response.result;
            } else {
                console.info(response);
            }
        });
    }
    $scope.EvaluatedorNames = "";
    $scope.EvaluatedorIds = "";
    var seluser1 = new SelUser({
        obj: $("#selectUser3"),
        selRole: "tea",//tea,stu,fam
        selType: "muti",//多选：muti,单选:single
        selArea: "colleague",//friend:我的好友,student:我的学生,stuparent:学生家长,colleague:我的同事,myclassmate:我的同学,teacher:我的老师,classmateparent:同学家长
        selValue: $("#EvaluatedorIds"),
        selText: $("#EvaluatedorNames"),
        callBackfn: function (ids, names) {
            //将先中的结果进行处理
            $scope.$apply(function() {
                $scope.Evaluatedor = ids;
                $scope.EvaluatedorNames = names;
                var arr1 = ids.split(",");
                var arr2 = names.split(",");
                for (var i = 0; i < arr1.length; i++) {
                    $scope.rows.push({
                        Name: arr2[i],
                        Id: arr1[i]
                    });
                }
                console.info($scope.rows);
            });
            
        }
    });
}]);