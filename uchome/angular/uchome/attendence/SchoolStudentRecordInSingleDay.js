function formatTime(date) {
    var year = date.getFullYear();
    var month = (date.getMonth() + 1);
    if (month < 10) month = "0" + month;
    var day = date.getDate();
    if (day < 10) day = "0" + day;
    var hour = date.getHours();
    if (hour < 10) hour = "0" + hour;
    var minutes = date.getMinutes();
    if (minutes < 10) minutes = "0" + minutes;
    var seconds = date.getSeconds();
    if (seconds < 10) seconds = "0" + seconds;
    return year + "-" + month + "-" + day + " " + hour + ":" + minutes+":"+seconds;
}

tempApp.controller("SchoolStudentRecordInSingleDay", ['$scope', '$http', "$filter", function ($scope, $http, $filter) {
    //年级班级
    $scope.Grades = [{ NJDM: 0, NJMC: "无年级数据" }];
    $scope.grade = $scope.Grades[0];
    $scope.Classes = [{ BJID: 0, XZBMC: "无班级数据" }];
    $scope.class = $scope.Classes[0];

    $http.get("/uchomenew/Attendence/GetSchoolGrades").success(function (response) {
        if (response.length > 0) {
            $scope.Grades = response;
            $scope.grade = $scope.Grades[0];
            $scope.GetGradeClass();
        }
    });
    $scope.GetGradeClass = function () {
        $http.get("/uchomenew/Attendence/GetSchoolClasses?NJDM=" + $scope.grade.NJDM).success(function (response) {
            if (response.length > 0) {
                $scope.Classes = response;
                $scope.class = $scope.Classes[0];
                $scope.GetClassStudents();
            }
        });
    }

    //时间
    var today = new Date();
    $scope.CheckTime = today.getFullYear() + "-" + (today.getMonth() + 1) + "-" + today.getDate();
    setTimeout(function() {
        $('#CheckTime').datetimepicker({
            language: 'zh-CN',
            format: 'yyyy-mm-dd',
            autoclose: true,
            todayBtn: true,
            weekStart: 1,
            todayHighlight: true,
            startView: 2,
            minView: 2,
            bootcssVer: 3
        }).on('changeDate', function(ev) {
            $scope.GetClassStudents();
        });
    }, 0);
    //类型
    $scope.CheckTypes = ["全部", "上午入校", "上午离校", "下午入校", "下午离校"];
    $scope.checkType = $scope.CheckTypes[0];
    //学生
    $scope.GetClassStudents = function () {
        if ($scope.class.BJID == 0) {
            alert("请选择有效的班级");
            return;
        }
        $http.get("/uchomenew/studentaffairs/GetClassStudentsList?classId=" + $scope.class.BJID).success(function (response) {
            if (response.flag) {
                $scope.ClassStudents = response.result;
                $scope.SearchRecord();
            } else {
                console.info(response);
            }
        });
    }
    $scope.SearchRecord = function () {
        if ($scope.class.BJID==0) {return;}
        var postData= {
            classId: $scope.class.BJID,
            startTime: $("#CheckTime").val() + " 00:00",
            endTime: $("#CheckTime").val() + " 23:59:59"
        }        
        $http.post("/uchomenew/Attendence/GetStudentRecordsByClass", postData).success(function (response) {
            if (response.length > 0) {
                $.each($scope.ClassStudents, function(idx, item) {
                    item.Records = $filter("filter")(response, { UID: item.XSID });
                });
            }
        });
    }
    $scope.FilterClass = function (records,first) {
        var returnType = "nameItem label ";
        if (typeof (records) == "undefined") {
            returnType += "label-danger";
        } else {
            switch ($scope.checkType) {
                case "全部":
                    returnType += records.length > 0 ? "label-success" : "label-danger"; break;
                case "上午入校":
                    returnType += ($filter("filter")(records, { "Comment": "上午迟到" })).length > 0 ? "label-warning" :
                    ($filter("filter")(records, { "Comment": "上午入校" }).length > 0 ?
                        "label-success" : "label-danger");
                    break;
                case "上午离校":
                    returnType += ($filter("filter")(records, { "Comment": "上午离校" })).length > 0 ?
                        "label-success" : "label-danger";
                    break;
                case "下午入校":
                    returnType += ($filter("filter")(records, { "Comment": "下午迟到" })).length > 0 ? "label-warning" :
                    ($filter("filter")(records, { "Comment": "下午入校" }).length > 0 ?
                        "label-success" : "label-danger");
                    break;
                case "下午离校":
                    returnType += $filter("filter")(records, { "Comment": "下午离校" }).length > 0 ?
                        "label-success" : "label-danger";
                    break;
                default:
                    returnType += "label-danger";
            }
        }
       
        if (first) {
            $scope.Statistic = {
                "default": 0,
                "warning": 0,
                "danger": 0
            }
        }
        switch (returnType) {
            case "nameItem label label-success":
                $scope.Statistic.default++;
                break;
            case "nameItem label label-warning":
                $scope.Statistic.warning++;
                break;
            case "nameItem label label-danger":
                $scope.Statistic.danger++;
                break;
        }
        return returnType;
    }
}]);

