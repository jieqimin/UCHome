tempApp.controller("studentask", ["$http", "$scope", function ($http, $scope) {
    $('#StartTime,#EndTime').datetimepicker({
        language: 'zh-CN',
        format: 'yyyy-mm-dd hh:ii',
        autoclose: true,
        todayBtn: true,
        weekStart: 1,
        todayHighlight: true,
        initalDate: new Date(),
        startView: 1
    }).on('changeDate', function () {
        var startDate = new Date($("#StartTime input").val());
        var endDate = new Date($("#EndTime input").val());
        if (!isNaN(endDate - startDate)) {
            $("#LeaveHours").val(((endDate - startDate) / 1000 / 60 / 60).toFixed(2));
            $scope.end = endDate;
            $scope.start = startDate;
        }
    });
    $scope.Submit = function (isValid) {
        if (isValid) {
            var postData= {
                "StartDate": $scope.start,
                "EndDate": $scope.end,
                "ApplyType": $scope.type,
                "LeaveHours": $("#LeaveHours").val(),
                "AskReason": $scope.LeaveReasonIntro,
                "teacherName": $scope.AuditorName,
                "teacherId": $scope.AuditorId
            }
            $http.post("/uchome/studentaskleave/StudentApply", postData).success(function(response) {
                if (response.flag) {
                    alert("提交成功");
                    $("#id-app-menu li[ui-sref='studentapplyList']").click();
                } else {
                    alert(response.Message);
                }
            });
        }
    }
}]);