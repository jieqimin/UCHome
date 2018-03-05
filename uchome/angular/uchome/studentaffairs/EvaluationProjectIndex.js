tempApp.controller("EvaluationProjectIndex", ["$scope", "$http", function ($scope, $http) {
    $scope.Systems = [{ Name: "请选择", Id: "0" }];
    $scope.ZtreeObj = {};
    var setting = {
        view: {
            showIcon: showIconForTree
        },
        data: {
            simpleData: { enable: false, idKey: "Id" },
            key: {
                name: "Name",
                children: "Properties"
            }
        },
        callback: {
            onClick: function (event, treeId, treeNode) {
                $scope.$apply(function () {
                    console.info(treeNode);
                });
            }
        },
        check: {
            enable: true,
            chkStyle: "checkbox",
            chkboxType: { "Y": "ps", "N": "ps" }
        }
    };
    $http.get("/uchome/Studentaffairs/GetAllSystemRecursion").success(function(response) {
        if (response.flag) {
            $scope.Systems = response.result;
        } else {
            console.info(response);
        }
    });
   
    $scope.ChangePropertyTree= function() {
        $scope.ZtreeObj=$.fn.zTree.init($("#treeDemo"), setting, $scope.System.Properties);
    }
    $scope.SaveProject = function () {
        $scope.warnList = [];
        $(".ng-valid-required").css("border-color", "#ccc");
        $(".ng-invalid-required").each(function (idx, item) {
            switch ($(item).attr("ng-model")) {
                case "Name":
                    $scope.warnList.push("项目名称必填");
                    break;
                case "System":
                    $scope.warnList.push("评价体系必选");
                    break;
            }
        });
        var checkedNodes = $scope.ZtreeObj.getCheckedNodes(true);
        if (checkedNodes.length == 0) {
            $scope.warnList.push("请至少选择一个评价项");
        }
        $(".ng-invalid-required").css("border-color", "red");
        if ($scope.warnList.length == 0) {
            var propertyIds = [];
            $.each(checkedNodes,function (idx, item) {
                if (item.level == 1) {
                    propertyIds.push({ SystemParentId:item.getParentNode().Id,SystemId:item.Id });
                }
            });
            var project = {
                Id:$("#project").val(),
                Name: $scope.Name,
                SystemId: $scope.System.Id,
                Describe: $scope.System.Describe,
                Type: $scope.evalutatedType,
                Circle: $scope.evalutatedCircle
            };
            
            $http.post("/uchome/studentAffairs/AddOrUpdateEvalutionProject", { model: project, propertyIds: propertyIds }).
                success(function (response) {
                    if (response.flag) {
                        $scope.warnList.push("保存成功");
                        $scope.Name = "";
                        $scope.System = {};
                        $scope.evalutatedType = 'student';
                        $scope.evalutatedCircle = 'day';
                        setTimeout(function () {
                            $scope.$apply(function() {
                                $scope.warnList = [];
                            });
                        },3000);
                    }

                });
        }

    }

    function showIconForTree(treeId, treeNode) {
        return !treeNode.isParent;
    };
}])