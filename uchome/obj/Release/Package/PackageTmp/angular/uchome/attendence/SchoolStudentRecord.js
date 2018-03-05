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

tempApp.controller("SchoolStudentRecord", ['$scope', '$http', function ($scope, $http) {
    //获取学校所有年级
    $http.get("/uchome/Attendence/GetSchoolGrades").success(function (response) {
        $("#grades").empty();
        if (response.length > 0) {
            $.each(response, function (idx, item) {
                $("<option>").text(item.NJMC).val(item.NJDM).appendTo($("#grades"));
                if (idx == 0) {
                    $scope.grade = item.NJDM;
                }
            });
        }
        $scope.GetClasses($scope.grade);
    });
    //获取学校所有班级
    $scope.GetClasses = function() {
        $http.get("/uchome/Attendence/GetSchoolClasses?NJDM=" + $scope.grade).success(function (response) {
            $("#classes").empty();
            $scope.class = null;
            if (response.length > 0) {
                $.each(response, function (idx, item) {
                    if (idx == 0) {
                        $scope.class = item.BJID;
                    }
                    $("<option>").text(item.XZBMC).val(item.BJID).appendTo($("#classes"));
                });
            }
            $scope.SearchRecord();
        });
    }

    var today = new Date();
    //startTime
    $scope.StartTime = today.getFullYear() + "-" + (today.getMonth() + 1) + "-" + "1 06:00";
    //Endtime
    $scope.EndTime = formatTime(today);
    $scope.SearchRecord = function () {
        if (!$scope.class) return;
        var postData= {
            classId: $scope.class,
            startTime: $scope.StartTime,
            endTime: $scope.EndTime
        }        
        $http.post("/uchome/Attendence/GetStudentRecordsByClass", postData).success(function (response) {
            var arr = [];
            if (response.length > 0) {
                $.each(response, function(idx,item) {
                    var itemarr = [];
                    itemarr.push(item.UName);
                    itemarr.push(item.Comment);
                    item.Date = new Date(parseInt(item.Date.replace("/Date(", "").replace(")/", ""), 10));
                    item.Date = formatTime(item.Date);
                    itemarr.push(item.Date);
                    if (item.DateBak != null) {
                        item.DateBak = new Date(parseInt(item.DateBak.replace("/Date(", "").replace(")/", ""), 10));
                        item.DateBak = formatTime(item.DateBak);
                    }
                    itemarr.push(item.DateBak);
                    arr.push(itemarr);
                });
            }
            $("#recordTable").dataTable({
                "destroy": true,
		"order":[[2,"desc"]],
                "data": arr
            });
        });
    }
}]);

