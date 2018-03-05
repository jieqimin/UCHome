tempApp.controller("EvaluationModel3t", ["$scope", "$http","$filter", function ($scope, $http,$filter) {

    $scope.transInfo = function (type, value) {
        switch (type) {
            case "Date":
                return value.getFullYear() + "-" + (value.getMonth() + 1) + "-" + (value.getDate());
            case "Time":
                value = parseInt(value.replace(/\D/igm, ""));
                value = new Date(value);
                return $filter("date")(value, "yyyy-MM-dd HH:mm");
            case "Records":
                var resultValue = "";
                if (typeof (value) == "undefined") return "无记录";
                if (value.length==0) return "无记录";
                $.each(value, function(idx, item) {
                    resultValue += (idx == 0 ? "" : ";") + item.StudentName + ":" + item.Score;
                });
                return resultValue;
            default:
                return "";
        }
    }
    
    $("#HappenDate").val($scope.transInfo('Date', new Date()));
    $scope.Classes = [{ Key: 0, Value: "无班级数据" }];
    $scope.selectedClass = $scope.Classes[0];
    $scope.ClassStudents = [];
    $scope.ProjectId = $("#ProjectId").val();
    $scope.AllProperty = [];
    $scope.AllScaleScore = 0;
    $scope.showModalType = 1;
    $scope.showModal = {};
    $http.get("/uchome/studentaffairs/GetBZRClasses").success(function (response) {
        if (response.flag) {
            if (response.result.length > 0) {
                $scope.selectedClass = response.result[0];
                $scope.Classes = response.result;
                $scope.GetClassStudents();
                $scope.GetClassRecords();
            }
        } else {
            console.info(response);
        }
    });
    $scope.GetClassStudents = function () {
        $http.get("/uchome/studentaffairs/GetClassStudentsList?classId=" + $scope.selectedClass.Key).success(function (response) {
            if (response.flag) {
                $scope.ClassStudents = response.result;
                $scope.GetClassRecords();
            } else {
                console.info(response);
            }
        });
    }
    $scope.GetClassRecords = function () {
        $scope.ClassRecords = [];
        if ($scope.selectedClass.Key == 0) {
            alert("请选择有效的班级数据");
            return;
        }
        var postData = {
            HappenDate: $("#HappenDate").val(),
            ClassId: $scope.selectedClass.Key,
            ProjectId: $scope.ProjectId
        }
        $http.post("/uchome/studentaffairs/GetClassStudentRecords", postData).success(function (response) {
            if (response.flag) {
                $scope.ClassRecords = response.result;
                $scope.SetPropertyRecord();
            } else {
                console.log(response);
            }
        });
    }
    $scope.SetPropertyRecord = function () {
        for (var i = 0; i < $scope.AllProperty.length; i++) {
            for (var j = 0; j < $scope.AllProperty[i].Property.length; j++) {
                $scope.AllProperty[i].Property[j].Records = $filter("filter")($scope.ClassRecords, { "PropertyId": $scope.AllProperty[i].Property[j].Id });
                if ($scope.AllProperty[i].Property[j].Id == $scope.showModal.Id) {
                    $scope.showModal = $scope.AllProperty[i].Property[j];
                }
            }
        }
    }
    $http.get("/uchome/studentaffairs/GetEvaluationProjectDetail?projectId=" + $scope.ProjectId).success(function (response) {
        response = eval("(" + response + ")");
        if (response.flag) {
            $scope.AllProperty = response.result.System.Property;
        } else {
            console.info(response);
        }
    });
    
    $("#HappenDate").datetimepicker({
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
        $scope.GetClassRecords();
    });
    $scope.ShowModal= function(item,type) {
        $scope.showModal = item;
        $scope.showModalType = type;
        $scope.resetAllStudentScore();
        $scope.AllScaleScore = 0;
        $("#myModal .modal-title").text($scope.selectedClass.Value+ "(" + $("#HappenDate").val() + ")");
        $("#myModal").modal("show");
    }

    $scope.DeleteRecord = function () {
        if (confirm("确认删除选中的记录？")) {
            var arr = $filter("filter")($scope.showModal.Records, { "selected": true });
            var ids = [];
            $.each(arr, function (idx, item) {
                ids.push(item.Id);
            });
            $http.post("/uchome/studentaffairs/DeleteEvaluationRecord", ids).success(function (response) {
                if (response.flag) {
                    alert("成功删除" + response.result + "条记录");
                    $scope.GetClassRecords();
                } else {
                    console.info(response);
                }
            });
        }
    }
    $scope.resetAllStudentScore= function() {
        for (var i = 0; i < $scope.ClassStudents.length; i++) {
            $scope.ClassStudents[i].selected = false;
        }
    }
    $scope.SaveRecord = function () {
        var arr = $filter("filter")($scope.ClassStudents, { "selected": true });
        if (arr.length > 0) {
            var postData = [];
            $.each(arr, function (idx, item) {
                var score = $("#" + item.XSID).val();
                if (score == 0) return true;
                var postItem = {
                    projectId: $scope.ProjectId,
                    PropertyId: $scope.showModal.Id,
                    ClassId: $scope.selectedClass.Key,
                    StudentId: item.XSID,
                    HappenDate: $("#HappenDate").val(),
                    Score: score+""
                };
                postData.push(postItem);
            });
            if (postData.length == 0) {
                alert("记分值不能为0");
                return;
            }
            $http.post("/uchome/studentaffairs/AddNewEvaluationRecord", postData).success(function (response) {
                if (response.flag) {
                    alert("成功保存" + response.result + "条记录");
                    $scope.resetAllStudentScore();
                    $scope.GetClassRecords();
                    $scope.AllScaleScore = 0;
                    $scope.showModalType = 1;
                } else {
                    console.info(response);
                }
            });
        }
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
    $scope.TodayDate = $scope.transInfo("Date", new Date());
    $scope.RowSelecteClick = function (item) {
        var date = $scope.transInfo("Time", item.HappenDate);
        date = $scope.transInfo("Date", new Date(date));

        if (date == $scope.TodayDate) {
            item.selected = !item.selected;
        } else {
            return;
        }
    }
}])