tempApp.controller("EvaluationModel2t", ["$scope", "$http","$filter", function ($scope, $http,$filter) {

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
                if (typeof (value)=="undefined")return "";
                $.each(value, function(idx, item) {
                    resultValue += (idx == 0 ? "" : ";") + item.ProjectPropertyName+":"+item.Score;
                });
                return resultValue;
            case "RecordsScore":
                var score = 0;
                if (typeof (value) == "undefined") return score;
                $.each(value, function (idx, item) {
                    score += item.Score;
                });
                return score;
            default:
                return "";
        }
    }
    
    $("#HappenDate").val($scope.transInfo('Date', new Date()));
    $scope.Classes = [{ Key: 0, Value: "无班级数据" }];
    $scope.selectedClass = $scope.Classes[0];
    $scope.ProjectId = $("#ProjectId").val();
    $scope.ClassStudents = [];
    $scope.AllProperty = [{ Name: "请选择分类", Property: [] }];
    $scope.selectedProperty = $scope.AllProperty[0];
    $scope.showModalType = 1;
    $scope.showModal = {};
    $scope.DisableSaveBtn = true;
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
                $scope.SetStudentRecord();
            } else {
                console.log(response);
            }
        });
    }
    $scope.SetStudentRecord = function () {
        
        for (var i = 0; i < $scope.ClassStudents.length; i++) {
            $scope.ClassStudents[i].Records = $filter("filter")($scope.ClassRecords, { "StudentNum": $scope.ClassStudents[i].XJH });
            if ($scope.ClassStudents[i].XSID == $scope.showModal.XSID) {
                $scope.showModal = $scope.ClassStudents[i];
            }
        }
    }
    $http.get("/uchome/studentaffairs/GetEvaluationProjectDetail?projectId=" + $scope.ProjectId).success(function (response) {
        response = eval("(" + response + ")");
        if (response.flag) {
            $scope.AllProperty = response.result.System.Property;
            $scope.selectedProperty = $scope.AllProperty[0];
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
        $scope.resetAllPropertyScore();
        $scope.selectedProperty = $scope.AllProperty[0];
        
        $("#myModal .modal-title").text(item.XM + "(" + $("#HappenDate").val() + ")");
        $("#myModal").modal("show");
    }
    $scope.CheckAllPropertyScore = function () {
        for (var i = 0; i < $scope.AllProperty.length; i++) {
            for (var j = 0; j < $scope.AllProperty[i].Property.length; j++) {
                if ($scope.AllProperty[i].Property[j].Score != 0) {
                    $scope.DisableSaveBtn = false;
                    return;
                }
            }
        }
        $scope.DisableSaveBtn = true;
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
    $scope.resetAllPropertyScore= function() {
        for (var i = 0; i < $scope.AllProperty.length; i++) {
            for (var j = 0; j < $scope.AllProperty[i].Property.length; j++) {
                $scope.AllProperty[i].Property[j].Score = 0;
            }
        }
        $scope.DisableSaveBtn = true;
    }
    $scope.SaveRecord= function() {
        var postData = [];
        $.each($scope.AllProperty, function (idx, item) {
            $.each(item.Property, function (idx2, item2) {
                var score = item2.Score;
                if (score == 0) return true;
                var postItem = {
                    projectId: $scope.ProjectId,
                    PropertyId: item2.Id,
                    ClassId: $scope.selectedClass.Key,
                    StudentId: $scope.showModal.XSID,
                    HappenDate: $("#HappenDate").val(),
                    Score: score+""
                };
                postData.push(postItem);
            });
        });
        $http.post("/uchome/studentaffairs/AddNewEvaluationRecord", postData).success(function (response) {
            if (response.flag) {
                alert("成功保存" + response.result + "条记录");
                $scope.GetClassRecords();
                $scope.resetAllPropertyScore();
                $scope.showModalType = 1;
            } else {
                console.info(response);
            }
        });

        
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