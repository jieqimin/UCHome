tempApp.controller("teacherIndex", ["$http", "$scope", "$timeout", function ($http, $scope, $timeout) {
    $scope.showModal={}
    $scope.DetailApply = function (applyid,audit) {
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
                if (audit) {
                    $scope.showModal.Audit = audit;
                    $("input[name='AuditSelect']").each(function (idx, item) {
                        $(item).removeAttr("checked");
                    });
                }
                $("#modal").modal('show');
            } else {
                alert("无法获取请假详情，请联系管理员");
                console.info(response);
            }
        });
    }
    $scope.AuditApply = function (applyid) {
        
        $scope.DetailApply(applyid,true);
        
        
    }
 
    $scope.TransTime = function (time) {
        time = parseInt(time.replace(/\D/igm, ""));
        time = new Date(time);
        time = time.getFullYear() + "年" + (time.getMonth() + 1) + "月" + time.getDate() + "日 " + time.getHours() + ":" + (time.getMinutes() < 10 ? "0" + time.getMinutes() : time.getMinutes());
        return time;
    }
    $scope.SaveAudit = function () {
        var postData = {
            applyId: $scope.showModal.ApplyId,
            auditResult: $scope.showModal.AuditResult,
            remark: $scope.showModal.AuditResult=="transe"?$("#selectUserName").val()+"|"+$("#selectuserId").val():""
        };
        $http.post("/uchome/studentaskleave/TeacherAudit",postData).success(function (response) {
            if (response.flag) {
                alert("保存成功");
                $("#modal").modal('hide');
                window.location.reload();
            } else {
                alert("保存失败");
                console.info(response);
            }
        });
        
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