tempApp.controller("StatisticByClass", ["$scope", "$http", "$filter", function ($scope, $http, $filter) {
    $scope.transInfo = function (type, value) {
        switch (type) {
            case "Date":
                return value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + (value.getDate());
        
            default:
                return "";
        }
    }
    $scope.ProjectId = $("#ProjectId").val();
    $scope.Grades = [{ NJDM: 0, NJMC: "无年级数据" }];
    $scope.grade = $scope.Grades[0];
    $scope.Classes = [];
    //开始时间结束时间选框
    $("#beginDate").val($scope.transInfo("Date", new Date(new Date().setDate(1))));
    $("#endDate").val($scope.transInfo("Date", new Date()));
    $http.get("/uchome/Attendence/GetSchoolGrades").success(function (response) {
        if (response.length > 0) {
            $scope.Grades = response;
            $scope.grade = $scope.Grades[0];
            $scope.GetGradeClass();
        }
    });
    $scope.GetGradeClass = function () {
        $http.get("/uchome/Attendence/GetSchoolClasses?NJDM=" + $scope.grade.NJDM).success(function (response) {
            if (response.length > 0) {
                $scope.Classes = response;
                $scope.GetStatisticDataByClass();
            }
        });
    }
    $scope.GetStatisticDataByClass = function()
    {
        var postData = {
            beginTime: $("#beginDate").val(),
            endTime: $("#endDate").val(),
            projectId: $scope.ProjectId,
            NJDM: $scope.grade.NJDM
        }
        $http.post("/uchome/studentaffairs/GetStatisticDataByClass", postData).success(function(response) {
            if (response.flag) {

                $.each($scope.Classes, function (idx, item) {
                    $.each(response.result, function (idx1, item1) {
                        var level = {
                            score: response.result[0].Average, Index: 1
                        }
                        if (level.score > item1.Average) {
                            level.Index++;
                        }
                        if (item.BJID == item1.BJID) {
                            item.Score = item1.Score;
                            item.StudentCount = item1.StudentCount;
                            item.Index = level.Index;
                            item.Average = item1.Average;
                        }
                    });
                });
                console.info($scope.Classes);
            } else {
                console.info(response);
            }
        });
    }
    $scope.ProduceExcel= function() {
        var excelObj = {
            TableName: $scope.grade.NJMC,
            Tbody: []
        }
        var tr = [];
        tr.push({ Value: "班级名称" });
        tr.push({ Value: "班级人数" });
        tr.push({ Value: "班级总记分" });
        tr.push({ Value: "平均记分" });
        tr.push({ Value: "年级排名(按平均分)" });
        excelObj.Tbody.push({ ExcelTds: tr });
        $.each($scope.Classes, function(idx, item) {
            var tr = [];
            tr.push({ Value: item.XZBMC });
            tr.push({ Value: item.StudentCount });
            tr.push({ Value: item.Score });
            tr.push({ Value: item.Average });
            tr.push({ Value: item.Index });
            excelObj.Tbody.push({ ExcelTds: tr });
        });
        $http.post("/uchome/studentaffairs/GetXLSFileByTable", {
            table: excelObj
        }).success(function (response) {
            if (response.flag) {
                window.open("/uchome/studentaffairs/GetXLSFileByName?fileName=" + response.result + "&relName="+$scope.grade.NJMC + "统计数据.xls");
            } else {
                console.info(response);
            }
        });
    }
    $("#beginDate,#endDate").datetimepicker({
        language: 'zh-CN',
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayBtn: true,
        weekStart: 1,
        todayHighlight: true,
        startView: 2,
        minView: 2,
        bootcssVer: 3
    }).on('changeDate', function (ev) {
        $scope.GetStatisticDataByClass();
    });
}])