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
tempApp.controller("StuRecordIndex", ['$scope', '$http', function ($scope, $http) {
    //获取班主任所有班级
    $http.get("/uchomenew/Attendence/GetTeaChargeClasses").success(function (response) {
        $("#classes").empty();
        if (response.length > 0) {
            $.each(response, function (idx, item) {
                if (idx == 0) {
                    $scope.class = item.classId;
                }
                $("<option>").text(item.className).val(item.classId).appendTo($("#classes"));
            });
            $scope.SearchRecord();
        }
    });
    
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
        $http.post("/uchomenew/Attendence/GetStudentRecordsByClass", postData).success(function (response) {
            var arr = [];
            if (response.length > 0) {
                $.each(response, function(idx,item) {
                    var itemarr = [];
                    itemarr.push(item.UName);
                    itemarr.push(item.Comment);
                    item.Date = new Date(parseInt(item.Date.replace("/Date(","").replace(")/", ""),10));
                    //item.Date = new Date(item.Date);
                    item.Date = formatTime(item.Date);
		    itemarr.push(item.Date);
		    if(item.DateBak!=null)
                    {item.DateBak = new Date(parseInt(item.DateBak.replace("/Date(","").replace(")/", ""),10));
                    //item.Date = new Date(item.DateBak);
                    item.DateBak = formatTime(item.DateBak);}
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

