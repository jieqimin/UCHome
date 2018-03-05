tempApp.controller("EvalutionStatisticByStudentT", ["$scope", "$http", "$filter", function ($scope, $http, $filter) {
    //转码翻译
    $scope.transInfo = function (type, value) {
        switch (type) {
            case "Date":
                return value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + (value.getDate());
            case "RecordsScore":
                var score = 0;
                if (typeof (value) == "undefined") return score;
                $.each(value, function (idx, item) {
                    score += item.Score;
                });
                return score;
            case "Level":
                var levelArr = $scope.LevelModel.LevelList.sort(function (a, b) {
                    return b.Score - a.Score;
                });
                for (var i = 0; i < levelArr.length; i++) {
                    if (value >= levelArr[i].Score) {
                        return levelArr[i].Name;
                    }
                }
                return $scope.LevelModel.DefaultBottomLevel;
            default:
                return "";
        }
    }
    //项目ID
    $scope.ProjectId = $("#ProjectId").val();
    //班主任班级下拉选框
    $scope.Classes = [{ Key: 0, Value: "无班级数据" }];
    $scope.selectedClass = $scope.Classes[0];
    $http.get("/uchome/studentaffairs/GetBZRClasses").success(function (response) {
        if (response.flag) {
            if (response.result.length > 0) {
                $scope.selectedClass = response.result[0];
                $scope.Classes = response.result;
                $scope.GetClassStudents();
            }
        } else {
            console.info(response);
        }
    });
    //开始时间结束时间选框
    $("#beginDate").val($scope.transInfo("Date", new Date(new Date().setDate(1))));
    $("#endDate").val($scope.transInfo("Date", new Date()));
    //班级学生
    $scope.GetClassStudents = function () {
        $http.get("/uchome/studentaffairs/GetClassStudentsList?classId=" + $scope.selectedClass.Key).success(function (response) {
            if (response.flag) {
                $scope.ClassStudents = response.result;
                $scope.GetClassStatisticData();
            } else {
                console.info(response);
            }
        });
    }
    //班级统计信息
    $scope.GetClassStatisticData = function () {
        if ($scope.selectedClass.Key == 0) {
            alert("请选择有效的班级");
            return;
        }
        var postData = {
            beginDate: $("#beginDate").val(),
            endDate: $("#endDate").val(),
            classId: $scope.selectedClass.Key,
            projectId: $scope.ProjectId
        }
        $http.post("/uchome/studentaffairs/GetStatisticDataByStudent", postData).success(function (response) {
            if (response.flag) {
                for (var i = 0; i < $scope.ClassStudents.length; i++) {
                    $scope.ClassStudents[i].Records = $filter("filter")(response.result, { StudentId: $scope.ClassStudents[i].XSID });
                }
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
        $scope.GetClassStatisticData();
    });
    //项目数据
    $scope.ProjectInfo = {};
    $http.get("/uchome/studentaffairs/GetEvaluationProjectDetail?projectId=" + $scope.ProjectId).success(function (response) {
        response = eval("(" + response + ")");
        if (response.flag) {
            $scope.ProjectInfo = response.result;
        } else {
            console.info(response);
        }
    });

    //弹出层
    $scope.ShowModal = function (item) {
        $.each($scope.ProjectInfo.System.Property, function (idx, itemObj) {
            itemObj.RowSpan = 0;
            $.each(itemObj.Property, function (idx2, itemObj2) {
                itemObj2.TitleShow = false;
                if ($filter("filter")(item.Records, { "PropertyId": itemObj2.Id }).length > 0) {
                    if (itemObj.RowSpan == 0) {
                        itemObj2.TitleShow = true;
                    }
                    itemObj.RowSpan++;
                }
            });
        });
        $scope.showModal = item;
        $("#myModal").modal("show");
    }

    //获取跨行数
    $scope.LevelModel = {
        TotalScore: 100,
        LevelList: [
        {
            Name: "优",
            Score: 90
        }, {
            Name: "良",
            Score: 80
        }, {
            Name: "中",
            Score: 70
        }],
        DefaultBottomLevel: "差",
        On: false
    }
    $scope.SetTotalScoreAndLevel = function () {
        $(".div-float").toggle();
    }
    $scope.AddNewLevel = function () {
        $scope.LevelModel.LevelList.push({ Name: "", Score: 0 });
    }
    $scope.SaveSetting = function () {
        $scope.LevelModel.On = !$scope.LevelModel.On;
        $(".div-float").toggle();
    }
    $scope.RemoveLevel = function (item) {
        $.each($scope.LevelModel.LevelList, function (idx, itemObj) {
            if (itemObj == item) {
                $scope.LevelModel.LevelList.splice(idx, 1);
                return;
            }
        });
    }
    $scope.ProduceExcel = function () {
       
        var excelObj = {
            TableName: $scope.selectedClass.Value,
            Tbody: []
        }
        var excelTrs = [];
        var excelTds = [];
        excelTds.push({ Value: "学生姓名" });
        excelTds.push({ Value: "学籍号" });
        excelTds.push({ Value: "记分总计" });
        if ($scope.LevelModel.On) {
            excelTds.push({ Value: "得分" });
            excelTds.push({ Value: "等级" });
        }
        excelTds.push({ Value: "记分类别" });
        excelTds.push({ Value: "记分项" });
        excelTds.push({ Value: "记分值" });
        excelTrs.push({ ExcelTds: excelTds });
        var allProperties = $scope.ProjectInfo.System.Property;
        $.each($scope.ClassStudents, function (idx, item) {
            var totalRowSpan = 0;
            $.each(allProperties, function (idx2, itemObj2) {
                itemObj2.RowSpan = 0;
                $.each(itemObj2.Property, function (idx3, itemObj3) {
                    itemObj3.TitleShow = false;
                    itemObj3.FirstHaveValueLine = false;
                    if ($filter("filter")(item.Records, { "PropertyId": itemObj3.Id }).length > 0) {
                        if (itemObj2.RowSpan == 0) {
                            itemObj3.TitleShow = true;
                        }
                        itemObj2.RowSpan++;
                        if (totalRowSpan == 0) {
                            itemObj3.FirstHaveValueLine = true;
                        }
                        totalRowSpan++;
                    }
                });
            });
            if (totalRowSpan == 0) {

                excelTds = [];

                excelTds.push({ Value: item.XM });
                excelTds.push({ Value: item.XJH });
                var score = $scope.transInfo("RecordsScore", item.Records);
                excelTds.push({ Value: score });
                if ($scope.LevelModel.On) {
                    excelTds.push({ Value: ($scope.LevelModel.TotalScore + score) });
                    excelTds.push({ Value: $scope.transInfo('Level', $scope.LevelModel.TotalScore + score) });
                }
                excelTrs.push({ ExcelTds: excelTds });
            } else {
                $.each(allProperties, function (idx2, itemObj2) {
                    $.each(itemObj2.Property, function (idx3, itemObj3) {
                        var itemArr = $filter("filter")(item.Records, { 'PropertyId': itemObj3.Id });
                        if (itemArr.length > 0) {
                            excelTds = [];
                            if (itemObj3.FirstHaveValueLine) {
                                excelTds.push({ Value: item.XM, RowSpan: totalRowSpan });
                                excelTds.push({ Value: item.XJH, RowSpan: totalRowSpan });

                                var score = $scope.transInfo("RecordsScore", item.Records);
                                excelTds.push({ Value: score, RowSpan: totalRowSpan });
                                if ($scope.LevelModel.On) {
                                    excelTds.push({ Value: $scope.LevelModel.TotalScore + score, RowSpan: totalRowSpan });
                                    excelTds.push({ Value: $scope.transInfo('Level', $scope.LevelModel.TotalScore + score), RowSpan: totalRowSpan });
                                }
                            }
                            if (itemObj3.TitleShow) {
                                excelTds.push({ Value: itemObj2.Name, RowSpan: itemObj2.RowSpan });
                            }
                            excelTds.push({ Value: itemObj3.Name });
                            excelTds.push({ Value: $scope.transInfo("RecordsScore", itemArr) });
                            excelTrs.push({ ExcelTds: excelTds });
                        }
                    });
                });
            }
        });
        excelObj.Tbody = excelTrs;
        $http.post("/uchome/studentaffairs/GetXLSFileByTable", {
            table: excelObj
        }).success(function (response) {
            if (response.flag) {
                window.open("/uchome/studentaffairs/GetXLSFileByName?fileName=" + response.result + "&relName=" + $scope.ProjectInfo.Name + "(" + $scope.selectedClass.Value + ")" + ".xls");
            } else {
                console.info(response);
            }
        });
    }
}])