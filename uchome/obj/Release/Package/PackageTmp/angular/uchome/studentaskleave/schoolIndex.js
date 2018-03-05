tempApp.controller("schoolIndex", ["$http", "$scope", "$timeout", function ($http, $scope, $timeout) {
    $scope.showModal = {}
    $scope.DetailApply = function (applyid) {
        $http.post("/uchome/studentaskleave/detailapply", { applyid: applyid }).success(function (response) {
            if (response.flag) {
                var modal = {}
                var applyInfo = response.Detail.ApplyInfo;
                modal.ApplyId = applyInfo.Id;
                modal.ApplyType = applyInfo.ApplyType;
                modal.ApplyerName = applyInfo.ApplyerName;
                modal.ApplyReason = applyInfo.AskReason;
                modal.StartTime = $scope.TransTime(applyInfo.StartDate);
                modal.EndTime = $scope.TransTime(applyInfo.EndDate);
                modal.LeaveHours = applyInfo.LeaveHours;
                modal.AuditInfo = [];
                $.each(response.Detail.AuditInfo, function (idx, item) {
                    modal.AuditInfo.push($scope.TransTime(item.CreateTime) + " " + item.ApplyerName + "提交至" + item.AuditorName + ": " + $scope.TransStatus(item.Status));
                });
                if (applyInfo.OutSchoolTime != null) {
                    modal.AuditInfo.push($scope.TransTime(applyInfo.OutSchoolTime) + " " + applyInfo.ApplyerName + "离校");
                }
                var studentInfo = response.Detail.StudentInfo;
                modal.ApplyerClass = studentInfo.NJMC + studentInfo.BJMC;
                $scope.showModal = modal;
                $("#modal").modal('show');
            } else {
                alert("无法获取请假详情，请联系管理员");
                console.info(response);
            }
        });
    }

    $scope.ApplyerLeaveSchool = function (applyid) {
        if (confirm("确认该学生离校？")) {
            $http.post("/uchome/studentaskleave/StudentLeaveSchool", { applyId: applyid }).success(function (response) {
                if (response.flag) {
                    $("#modal").modal('hide');
                    alert("确认成功");
                    window.location.reload();
                } else {
                    alert("确认失败，请联系管理员");
                    console.info(response);
                }
            });
        }
        
    }
    $scope.TransTime = function (time) {
        time = parseInt(time.replace(/\D/igm, ""));
        time = new Date(time);
        time = time.getFullYear() + "年" + (time.getMonth() + 1) + "月" + time.getDate() + "日 " + time.getHours() + ":" + (time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes());
        return time;
    }
    $scope.TransStatus = function (status) {
        switch (status) {
            case 0:
                return "待审";
            case 1:
                return "已转交";
            case 2:
                return "已同意";
            case 3:
                return "拒绝";
            case 4:
                return "审核中";
        }
    }
}])