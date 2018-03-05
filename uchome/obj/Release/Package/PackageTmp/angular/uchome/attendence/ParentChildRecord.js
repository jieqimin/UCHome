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

tempApp.controller("ParentChildRecord", ['$scope', '$http', function ($scope, $http) {

    
    var today = new Date();
    //startTime
    $scope.StartTime = today.getFullYear() + "-" + (today.getMonth() + 1) + "-" + "1 06:00";
    //Endtime
    $scope.EndTime = formatTime(today);
    $scope.SearchRecord = function () {
       
        var postData= {
            startTime: $scope.StartTime,
            endTime: $scope.EndTime
        }        
        $http.post("/uchomenew/Attendence/GetStudentRecordsByParent", postData).success(function (response) {
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
                "order": [[2, "desc"]],
                "data": arr
            });
        });
    }
}]);

