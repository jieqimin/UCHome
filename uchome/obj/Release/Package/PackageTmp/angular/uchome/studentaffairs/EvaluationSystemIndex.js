
tempApp.controller("EvaluationSystemIndex", ["$scope", "$http", function ($scope, $http) {
    var setting = {
        view: {
            showIcon: showIconForTree
        },
        data: {
            simpleData: { enable: true, pIdKey: "pId",idKey:"Id" },
            key: {
                name: "Name"
            }
        },
        callback: {
            onClick: function (event, treeId, treeNode) {
                $scope.$apply(function() {
                    $scope.showSystem = treeNode;
                    $scope.switchView(false);
                    $scope.EditBtn = true;
                });
                
            }
        }
    };

    $scope.EditBtn = false;
    $scope.showSystem = {};
    $scope.zNodes = [];
    function showIconForTree(treeId, treeNode) {
        return !treeNode.isParent;
    };
    $scope.AddChildSystem= function() {
        var childSystem = {};
        childSystem.ParentId = $scope.showSystem.Id;
        $scope.showSystem = childSystem;
        $scope.switchView(true);
    }
    $scope.GetAllSystemNodes = function (treeId) {
        $http.get("/uchome/StudentAffairs/GetAllEvaluationSystem").success(function (response) {
            if (response.flag) {
                $scope.zNodes = response.result;
                var maxOrder = 0;
                if (treeId=="max") {
                    $.each($scope.zNodes, function(idx, item) {
                        if (item.Order > maxOrder) {
                            maxOrder = item.Order;
                            treeId = item.pId;
                        }
                    });
                }
                $scope.initTree(treeId);
            } else {
                console.info(response);
            }
        });
    }
    $scope.initTree = function (treeId) {
        var treeObj = $.fn.zTree.init($("#treeDemo"), setting, $scope.zNodes);
        var nodes = treeObj.getNodes();
        if (nodes.length > 0) {
            var node = treeObj.getNodesByParam("Id", treeId);
            if (node.length == 0) {
                node = nodes[0];
            } else {
                node = node[0];
            }
            treeObj.expandNode(node, true, false, true);
            treeObj.selectNode(node, false, false);
            $scope.showSystem = node;
            $scope.switchView(false);
            $scope.EditBtn = true;
        } else {
            $scope.addSystem = false;
        }
    }
    $scope.switchView = function ( edit) {
        $scope.edit = edit;
        $scope.addSystem = true;
        $scope.changeOrder = false;
    }
    $scope.ToUp= function() {
        var preNode = $scope.showSystem.getPreNode();
        $scope.ToUpdateChangeOrders([preNode, $scope.showSystem]);
    }
    $scope.ToDown = function () {
        var nextNode = $scope.showSystem.getNextNode();
        $scope.ToUpdateChangeOrders([nextNode, $scope.showSystem]);
    }
    $scope.ToUpdateChangeOrders = function (models) {
        var temp = models[0].Order;
        models[0].Order = models[1].Order;
        models[1].Order = temp;
        $http.post("/uchome/StudentAffairs/UpdateEvaluationSystems", { models: models }).success(function (response) {
            if (response.flag) {
                $scope.GetAllSystemNodes($scope.showSystem.Id);
            } else {
                console.info(response);
            }
        });
    }
    $scope.ToAdd = function () {
        if ($scope.showSystem.Name == "") {

        }
        if (typeof ($scope.showSystem.Id) != 'undefined') {
            $scope.ToUpdate();
            return;
        }
        $http.post("/uchome/StudentAffairs/AddEvaluationSystem", $scope.showSystem).success(function (response) {
            if (response.flag) {
                $scope.GetAllSystemNodes("max");
            } else {
                console.info(response);
            }
        });
    }
    $scope.deleteSystem = function () {
        if(confirm("确认删除该配置项？删除后，改配置项所有相关的纪录都将删除")){
        $http.post("/uchome/StudentAffairs/DeleteEvaluationSystem", { id: $scope.showSystem.Id }).success(function (response) {
            if (response.flag) {
                if (!$scope.showSystem.isParent) {
                    $scope.GetAllSystemNodes($scope.showSystem.pId);
                } else {
                    $scope.GetAllSystemNodes();
                }
            } else {
                console.info(response);
            }
        });
        }
    }
    $scope.ToUpdate= function() {
        $http.post("/uchome/StudentAffairs/UpdateEvaluationSystem", $scope.showSystem).success(function (response) {
            if (response.flag) {
                $scope.GetAllSystemNodes($scope.showSystem.Id);
            } else {
                console.info(response);
            }
        });
    }
    $scope.GetAllSystemNodes();
    $scope.AddSystem= function() {
        $scope.showSystem = {};
        $scope.switchView(true);
    }
    $scope.switchView(false);
    $scope.warnLabel = "<span>aaaa</span>";

}]);

//-->
