//tempApp.directive('signlessInteger', [function() {
    
//}]);
tempApp.factory("TaskModuleService", ["$http", "$q", function ($http, $q) {
    return {
        GetAllModules: function () {
            var defer = $q.defer();
            $http.get("/Uchome/TaskIntegral/GetAllModules").success(function (response) {
                defer.resolve(response);
            });
            return defer.promise;
        }
    };
}]);
tempApp.controller("TaskIntegral", ['$scope', '$http', "TaskModuleService", function ($scope, $http, TaskModuleService) {
    $scope.Roles = [{ key: "t", value: "教师" }, { key: "s", value: "学生" }, { key: "p", value: "家长" }];
    $scope.Cycles = [{ key: "day", value: "天" }, { key: "week", value: "周" }, { key: "month", value: "月" }, {key:"once",value:"一次性任务"}];
    $scope.QueryChanged = false;
    var setting = {
        edit: {
            enable: true,
            showRemoveBtn: setRemoveBtn,
            showRenameBtn: setRenameBtn,
            removeTitle: "删除",
            renameTitle: "编辑"
        },
        view: {
            showLine: false,
            showIcon: false,
            selectedMulti: false,
            dblClickExpand: false,
            addDiyDom: addDiyDom,
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom
        },
        data: {
            simpleData: {
                enable: true,
                rootPId: 0
            }
        },
        callback: {
            beforeClick: beforeClick,
            onClick: nodeClick,
            beforeEditName: beforeEditName,
            beforeDbClick: true,
            onDblClick: nodeDbClick,
            beforeDrag: beforeDrag,
            beforeDrop: beforeDrop,
            beforeRemove: beforeRemove
}
    };
    $scope.RefreshTree = function (treeId,treeNode) {
        TaskModuleService.GetAllModules().then(function (response) {
            var zNodes = formatNodes(response);
            var treeObj = $("#zTreeObj");
            var zTree = $.fn.zTree.init(treeObj, setting, zNodes);
            if (treeId != "") {
                var node = zTree.getNodeByParam("id", treeId, null);
                zTree.selectNode(node);
                $scope.Model = angular.copy(node);
                $scope.Model.view = true;
            }
            if (treeNode != null) {
                //zTree.expandNode(treeNode, true, false, true);
                treeNode = zTree.getNodeByParam("id", treeNode.id, null);
                if (treeNode.children == null || treeNode.children.length == 0) {
                    treeNode = treeNode.getParentNode();
                }
                zTree.expandNode(treeNode, true, false, true);
            }
            treeObj.hover(function () {
                if (!treeObj.hasClass("showIcon")) {
                    treeObj.addClass("showIcon");
                }
            }, function () {
                treeObj.removeClass("showIcon");
            });
        });

    }
    $scope.RefreshTree("",null);
    $scope.checkTaskModel = function (node) {
        if (node.levelName == "任务模型") {

        }
        return false;
    }
    $scope.EditModel = function () {
        $scope.Model.view = false;
    }
    $scope.changeStatus = function () {
        if ($scope.Model.levelName == "任务分类") {
            $scope.Model.TaskTypeStatus = $scope.Model.TaskTypeStatus == "1" ? "0" : "1";
        }
        if ($scope.Model.levelName == "任务") {
            $scope.Model.TaskStatus = $scope.Model.TaskStatus == "1" ? "0" : "1";
        }
    }
    $scope.changeModalStatus= function() {
        if ($scope.Modal.levelName == "任务分类") {
            $scope.Modal.TaskTypeStatus = $scope.Modal.TaskTypeStatus == "1" ? "0" : "1";
        }
        if ($scope.Modal.levelName == "任务") {
            $scope.Modal.TaskStatus = $scope.Modal.TaskStatus == "1" ? "0" : "1";
        }
    }
    $scope.SaveModel = function (valid,node) {
        if (valid) {
            //var node = $scope.Modal;
            var postData = {};
            //node.id = Guid.NewGuid();
            if (node.levelName == "任务模型") {
                postData = {
                    PKID: node.id,
                    ModuleName: node.name,
                    Porder: node.Porder
                }
                $http.post("/uchome/taskintegral/savemodule", postData).success(function (response) {
                    if (response.flag) {
                        $scope.RefreshTree(response.PKID,null);
                    } else {
                        console.info(response.message);
                        alert("保存失败");
                    }
                });
            }
            if (node.levelName == "任务分类") {
                postData = {
                    PKID: node.id,
                    MuduleID: node.pId,
                    TaskTypeName: node.name,
                    Porder: node.Porder,
                    TaskRole: node.TaskRole,
                    TaskTypeStatus: node.TaskTypeStatus,
                    TaskTypeScore: node.TaskTypeScore+""
                };
                $http.post("/uchome/taskintegral/saveTasktype", postData).success(function (response) {
                    if (response.flag) {
                        $scope.RefreshTree(response.PKID,null);
                    } else {
                        console.info(response.message);
                        alert("保存失败");
                    }
                });

            }
            if (node.levelName == "任务") {
                postData = {
                    PKID: node.id,
                    TaskTypeID: node.pId,
                    TaskName: node.name,
                    Porder: node.Porder,
                    TaskScore: node.TaskScore+"",
                    TaskStatus: node.TaskStatus+"",
                    TaskUrl: node.TaskUrl,
                    TaskCycle: node.TaskCycle
                };
                $http.post("/uchome/taskintegral/saveTask", postData).success(function (response) {
                    if (response.flag) {
                        $scope.RefreshTree(response.PKID,null);
                    } else {
                        console.info(response.message);
                        alert("保存失败");
                    }
                });
            }
            $("#demoModal").modal('hide');
        }
        
    }
    $scope.SaveChangedQuery= function() {
        var zTree = $.fn.zTree.getZTreeObj("zTreeObj");
        var nodes = zTree.transformToArray(zTree.getNodes());
        var postData = [];
        if (nodes.length > 0) {
            $.each(nodes, function(idx, item) {
                if (item.Porder != item.getIndex()) {
                    postData.push({ "Id": item.id, "order": item.getIndex(), "level": (item.levelName == "任务模型" ? 1 : (item.levelName == "任务分类" ? 2 : 3)) });
                }
            });
        }
        $http.post("/uchome/taskintegral/SaveChangedQuery", postData).success(function(response) {
            $scope.RefreshTree("", null);
            $scope.QueryChanged = false;
        });
    }

    ///////////////////////////////////////////////////////////////////////////////
    

    //格式化任务Json
    function formatNodes(obj) {

        var nodes = [];
        if (obj.length > 0) {
            $.each(obj, function (idx, item) {
                var node = {
                    id: item.PKID,
                    pId: 0,
                    name: item.ModuleName,
                    Porder: item.Porder,
                    levelName: "任务模型"
                }
                nodes.push(node);
                if (item.UCHome_TaskType.length > 0) {
                    $.each(item.UCHome_TaskType, function (idx2, item2) {
                        node = {
                            id: item2.PKID,
                            pId: item2.MuduleID,
                            name: item2.TaskTypeName,
                            Porder: item2.Porder,
                            TaskRole: $.trim(item2.TaskRole),
                            TaskTypeScore: item2.TaskTypeScore,
                            TaskTypeStatus: $.trim(item2.TaskTypeStatus),
                            levelName: "任务分类"

                        };
                        nodes.push(node);
                        if (item2.UCHome_Task.length > 0) {
                            $.each(item2.UCHome_Task, function (idx3, item3) {
                                node = {
                                    id: item3.PKID,
                                    pId: item3.TaskTypeID,
                                    name: item3.TaskName,
                                    Porder: item3.Porder,
                                    TaskCycle: item3.TaskCycle,
                                    TaskScore: item3.TaskScore,
                                    TaskStatus: $.trim(item3.TaskStatus),
                                    TaskUrl: item3.TaskUrl,
                                    levelName: "任务"
                                };
                                nodes.push(node);
                            });
                        }
                    });
                }
            });
        }
        var treeRoot = {
            id: 0,
            pId: "",
            name: "任务根节点",
            open: true,
            Porder:0
        };
        nodes.push(treeRoot);
        return nodes;
    }
    //设置非父节点可删除
    function setRemoveBtn(treeId, treeNodes) {
        if (treeNodes.level == 0) return false;
        return !treeNodes.isParent;
    }
    //设置非根节点可编辑
    function setRenameBtn(treeId, treeNodes) {
        return treeNodes.level > 0;
    }
    //更改zTree样式
    function addDiyDom(treeId, treeNode) {
        var spaceWidth = 5;
        var switchObj = $("#" + treeNode.tId + "_switch"),
        icoObj = $("#" + treeNode.tId + "_ico");
        switchObj.remove();
        icoObj.before(switchObj);

        if (treeNode.level > 1) {
            var spaceStr = "<span style='display: inline-block;width:" + (spaceWidth * treeNode.level) + "px'></span>";
            switchObj.before(spaceStr);
        }
    }
    //设置编辑节点
    function beforeEditName(treeId, treeNode) {

        $scope.$apply(function () {
                $scope.Model = angular.copy(treeNode);
                $scope.Model.view = false;
            });

        return false;
    }
    //双击事件
    function nodeDbClick(event, treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj("zTreeObj");
        zTree.expandNode(treeNode, !treeNode.open);
    }
    //设置非根节点可单击=》预览详情
    function beforeClick(treeId, treeNode) {
        if (treeNode.level == 0) {
            return false;
        }
        return true;
    }
    //单击事件=》预览详情
    function nodeClick(event, treeId, treeNode) {
        $scope.$apply(function () {
            $scope.Model = angular.copy(treeNode);
            $scope.Model.view = true;
        });
    }
    //增加（增加节点）控件
    function addHoverDom(treeId, treeNode) {
        if (treeNode.level != 1 && treeNode.level != 2 && treeNode.level != 0) {
            return false;
        }
        var title = treeNode.level == 0 ? "添加子任务模型" : (treeNode.level == 1 ? "添加子任务分类" : "添加子任务");
        var sObj = $("#" + treeNode.tId + "_span");
        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;
        var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
            + "' title='" + title + "' onfocus='this.blur();'></span>";
        sObj.after(addStr);
        var btn = $("#addBtn_" + treeNode.tId);
        if (btn) btn.bind("click", { treeNode: treeNode }, addNode);
        //function () {
        //添加节点
        //var zTree = $.fn.zTree.getZTreeObj("treeDemo");
        //zTree.addNodes(treeNode, { id: (100 + newCount), pId: treeNode.id, name: "new node" + (newCount++) });
        return false;
        //});
    };
    //移除控件范围样式调整
    function removeHoverDom(treeId, treeNode) {
        $("#addBtn_" + treeNode.tId).unbind().remove();
    };
    //非根节点不可拖动
    function beforeDrag(treeId, treeNodes) {
        return treeNodes[0].level > 0;
    }
    //只能拖动到相同父节点下面
    function beforeDrop(targetId, treeNodes, targetNode, moveType, isCopy) {
        if (isCopy) {
            alert("无法复制");
            return false;
        }
        if (moveType == "prev" || moveType == "next") {//当移动到目标节点之前或者之后时
            if (targetNode.getParentNode().tId == treeNodes[0].getParentNode().tId) {
                $scope.$apply(function() {
                    $scope.QueryChanged = true;
                });
                return true;
            }
        }
        alert("只能在同父节点下进行移动");
        return false;


    }
    //增加节点
    function addNode(e) {
        $scope.$apply(function () {
            $scope.parentLevel = e.data.treeNode.level;
            $scope.Modal = {
                levelName: $scope.parentLevel == 0 ? "任务模型" : ($scope.parentLevel == 1 ? "任务分类" : "任务"),
                Porder: e.data.treeNode.children == null ? 0 : e.data.treeNode.children.length,
                ParentName: e.data.treeNode.name,
                pId: e.data.treeNode.id,
                TaskTypeStatus: 0,
                TaskStatus: 0,
                TaskCycle:"day",
                TaskRole:"t"
            };
            $("#demoModal").modal();
        });
    }
    //删除节点之前
    function beforeRemove(treeId, treeNode) {
        var postData = {};
        if (treeNode.levelName == "任务模型") {
            postData = {
                PKID: treeNode.id,
                level:"1"
            }
        }
        if (treeNode.levelName == "任务分类") {
            postData = {
                PKID: treeNode.id,
                level: "2"
            };
        }
        if (treeNode.levelName == "任务") {
            postData = {
                PKID: treeNode.id,
                level:"3"
            };
        }
        $http.post("/uchome/taskintegral/delTaskModual", postData).success(function (response) {
            if (response.flag) {
                $scope.RefreshTree("",treeNode.getParentNode());
                
            } else {
                console.info(response.message);
                alert("保存失败");
            }
        });
        return false;
    }
    
    ///////////////////////////////////////////////////////////////////////////
}]);

