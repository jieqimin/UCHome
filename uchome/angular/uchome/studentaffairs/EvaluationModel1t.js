﻿
tempApp.controller("EvaluationModel1t", ["$scope", "$http", "$filter", function ($scope, $http, $filter) {
    $scope.transInfo = function (type, value) {
        switch (type) {
            case "Date":
                return value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + (value.getDate());
            case "Time":
                value = parseInt(value.replace(/\D/igm, ""));
                value = new Date(value);
                value = value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + value.getDate();
                return value;
                
            default:
                return "";
        }
    }
    $scope.projectId = $("#ProjectId").val();
    $scope.Classes = [{ Key: 0, Value: "无班级数据" }];
    $scope.selectedClass = $scope.Classes[0];
    $scope.beginDate = $scope.transInfo("Date", new Date(new Date().setDate(1)));
    $scope.endDate = $scope.transInfo("Date", new Date());
    $scope.Records = [];
    $scope.dtOptions = {
        "aaSorting": [[4, "desc"]]
    };
    $scope.classStudents = [];
    $scope.ProjectEvaluationItems = [{Name:"无选项",Property:[{Id:0,Name:"无选项"}]}];
    $scope.EvalutionItems = $scope.ProjectEvaluationItems[0];
    $scope.SelectedEvaluationItem=$scope.EvalutionItems[0];
    $scope.RecordDate = $scope.transInfo("Date", new Date());
 
    //班主任班级
    $http.get("/uchome/studentaffairs/GetBZRClasses").success(function (response) {
        if (response.flag) {
            if (response.result.length > 0) {
                $scope.selectedClass = response.result[0];
                $scope.Classes = response.result;
                $scope.SearchList();
            }
        } else {
            console.info(response);
        }
    });
    $scope.SearchList = function () {
        if ($scope.selectedClass.Key == 0) {
            angular.element("#warnSpan").text("请选择有效的班级").show();
            setTimeout(function () {
                $("#warnSpan").fadeOut();
            }, 3000);
            return;
        }
        var startDate = new Date($("#beginDate").val()).setHours(0);
        var endDate = new Date($("#endDate").val()).setHours(0);
        if (endDate - startDate < 0) {
            $("#warnSpan").text("结束时间应>=开始时间").show();
            setTimeout(function () {
                $("#warnSpan").fadeOut();
            }, 3000);
            return;
        }
        var postData= {
            classId: $scope.selectedClass.Key,
            beginDate: new Date(startDate),
            endDate: new Date(endDate),
            projectId:$scope.projectId
        }
        $http.post("/uchome/studentaffairs/GetClassEvaluationRecord", postData).success(function(response) {
            if (response.flag) {
                $scope.Records = response.result;
            } else {
                console.info(response);
            }
        });
    }
    $http.get("/uchome/studentAffairs/GetEvaluationProjectDetail?projectId=" + $scope.projectId).success(function (response) {
            response = eval("(" + response + ")");
            if (response.flag) {
                $scope.ProjectEvaluationItems = response.result.System.Property;
                $scope.EvalutionItems = $scope.ProjectEvaluationItems.length > 0 ? $scope.ProjectEvaluationItems[0] : {};
                $scope.SelectedEvaluationItem = $scope.EvalutionItems.Property.length > 0 ? $scope.EvalutionItems.Property[0]: 0;
            } else {
                console.info(response);
            }
        });
    $scope.AddNewCordClick= function() {
        $('#myModal').modal('show');
        $("#studentList").prev().text("选择学生");
        $("#studentList").slideUp(0);
        $scope.AllScaleScore = 0;
        $http.get("/uchome/studentaffairs/GetClassStudentsList?classId=" + $scope.selectedClass.Key).success(function(response) {
            if (response.flag) {
                $scope.classStudents = response.result;
            } else {
                console.info(response);
            }
        });
    }
    $scope.ShowStudents = function () {
        if ($("#studentList").is(":hidden")) {
            $("#studentList").prev().text("收起列表");
            $("#studentList").slideDown();
        } else {
            $("#studentList").prev().text("选择学生");
            $("#studentList").slideUp();
        }
    }
    $scope.SaveRecord= function() {
        var arr = $filter("filter")($scope.classStudents, { "selected": true });
        if (arr.length > 0) {
            var postData = [];
            $.each(arr, function (idx, item) {
                var score = $("#" + item.XSID).val();
                if (score == 0) return true;
                var postItem = {
                    projectId: $scope.projectId,
                    PropertyId: $scope.SelectedEvaluationItem.Id,
                    ClassId: $scope.selectedClass.Key,
                    StudentId: item.XSID,
                    HappenDate: $("#RecordDate").val(),
                    Score: score
                };
                postData.push(postItem);
            });
            $http.post("/uchome/studentaffairs/AddNewEvaluationRecord",postData).success(function (response) {
                if (response.flag) {
                    alert("成功保存" + response.result + "条记录");
                    $scope.SearchList();
                } else {
                    console.info(response);
                }
            });
          
        }
    }
    $scope.DeleteRecord = function () {
        if (confirm("确认删除选中的记录？")) {
            var arr = $filter("filter")($scope.Records, { "selected": true });
            var ids = [];
            $.each(arr, function (idx, item) {
                ids.push(item.Id);
            });
            $http.post("/uchome/studentaffairs/DeleteEvaluationRecord", ids).success(function (response) {
                if (response.flag) {
                    alert("成功删除" + response.result + "条记录");
                    $scope.SearchList();
                } else {
                    console.info(response);
                }
            });
        }
        
    }
    $scope.TodayDate = $scope.transInfo("Date", new Date());
    $scope.RowSelecteClick = function (item) {
        var date = $scope.transInfo("Time", item.HappenDate);
        if (date == $scope.TodayDate) {
            item.selected = !item.selected;
        } else {
            return;
        }
    }
}])