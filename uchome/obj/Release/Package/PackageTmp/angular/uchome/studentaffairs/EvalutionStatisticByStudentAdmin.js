tempApp.controller("EvalutionStatisticByStudentAdmin", ["$scope", "$http", "$filter", function ($scope, $http, $filter) {
    

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
    //年级班级下拉选框
    $scope.Grades = [{ NJDM: 0, NJMC: "无年级数据" }];
    $scope.grade = $scope.Grades[0];
    $scope.Classes = [{ BJID: 0, XZBMC: "无班级数据" }];
    $scope.selectedClass = $scope.Classes[0];
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
                $scope.selectedClass = $scope.Classes[0];
                $scope.GetClassStudents();
            }
        });
    }
    //开始时间结束时间选框
    $("#beginDate").val($scope.transInfo("Date", new Date(new Date().setDate(1))));
    $("#endDate").val($scope.transInfo("Date", new Date()));
    //班级学生
    $scope.GetClassStudents = function () {
        $http.get("/uchome/studentaffairs/GetClassStudentsList?classId=" + $scope.selectedClass.BJID).success(function (response) {
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
        if ($scope.selectedClass.BJID == 0) {
            alert("请选择有效的班级");
            return;
        }
        var postData = {
            beginDate: $("#beginDate").val(),
            endDate: $("#endDate").val(),
            classId: $scope.selectedClass.BJID,
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
        var classIds = [];
        $("#exportModal input:checkbox").each(function(idx, item) {
            if (item.checked) {
                classIds.push(item.value);
            }
        });
        $http.post("/uchome/studentaffairs/GetStatisticFile", {
            levelModel: $scope.LevelModel,
            beginTime: $("#beginDate").val(),
            endTime: $("#endDate").val(),
            classIds: classIds,
            projectId: $scope.ProjectId

        }).success(function (response) {
            if (response.flag) {
                window.open("/uchome/studentaffairs/GetXLSFileByName?fileName=" + response.result + "&relName=" + $scope.ProjectInfo.Name + ".xls");
            } else {
                console.info(response);
            }
        });
    }

    $scope.ShowSelectClassModal = function () {
        $("#exportModal").modal("show");
        $("#exportModal input:checkbox").removeAttr("checked");
    }

}])