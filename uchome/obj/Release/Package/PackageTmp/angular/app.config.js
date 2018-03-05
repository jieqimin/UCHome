"use strict"
var tempApp = angular.module("appCenter", ["ui.router", "oc.lazyLoad", "datatables"])
.config(["$provide", "$compileProvider", "$controllerProvider", "$filterProvider",
        function ($provide, $compileProvider, $controllerProvider, $filterProvider) {
            tempApp.controller = $controllerProvider.register;
            tempApp.directive = $compileProvider.register;
            tempApp.filter = $filterProvider.register;
            tempApp.factory = $provide.factory;
            tempApp.service = $provide.service;
            tempApp.constant = $provide.constant;
        }]);
tempApp.directive("treeView", function () {
    return {
        restrice: 'AE',
        replace: true,
        template: '<a href="#" ><span val="zj.Id" ng-click="selzj(zj.Id,zj.Name)">{{zj.Name}}</span><ul ng-if="zj.ChildNodeBookModel.length"><li ng-repeat="zj in zj.ChildNodeBookModel"><tree-View></tree-View></li></ul></a>'
    };
});
tempApp.directive("selKnowledge", function($http) {
    var obj = '<div class="bs-example" data-example-id="single-button-dropdown">';
    obj += '<div class="btn-group"> ';
    obj += '<button id="btnselxd" ng-click="toggledrop('+"'select_xd'"+')" class="btn btn-primary dropdown-toggle" aria-expanded="false" aria-haspopup="true" type="button" data-toggle="dropdown" ng-init="selxdvalue=\'选择学段\'">{{selxdvalue}} <span class="caret"></span></button>';
    obj += '<ul class="dropdown-menu" id="select_xd">';
    obj += '<li ng-repeat="xd in xds"><a href="#" val="xd.value" ng-click="selxd(xd.value,xd.Name)">{{xd.Name}}</a></li>';
    obj += '</ul>  ';
    obj += '  </div><!-- /btn-group -->  ';
    obj += '  <div class="btn-group"> ';
    obj += ' <button  id="btnselxk" ng-click="toggledrop(' + "'select_xk'" + ')" class="btn btn-success dropdown-toggle" aria-expanded="false" aria-haspopup="true" type="button" data-toggle="dropdown" ng-init="selxkvalue=\'选择学科\'">{{selxkvalue}}<span class="caret"></span></button>';
    obj += ' <ul class="dropdown-menu" id="select_xk">';
    obj += '<li ng-repeat="xk in xks"><a href="#" val="xk.Value" ng-click="selxk(xk.Value,xk.DisplayName)">{{xk.DisplayName}}</a></li>';
    obj += ' </ul>  ';
    obj += '  </div><!-- /btn-group -->  ';
    obj += '  <div class="btn-group"> ';
    obj += ' <button id="btnselbb" ng-click="toggledrop(' + "'select_bb'" + ')" class="btn btn-info dropdown-toggle" aria-expanded="false" aria-haspopup="true" type="button" data-toggle="dropdown" ng-init="selbbvalue=\'选择版本\'">{{selbbvalue}} <span class="caret"></span></button>';
    obj += ' <ul class="dropdown-menu" id="select_bb">  ';
    obj += '<li ng-repeat="bb in bbs"><a href="#" val="bb.Value" ng-click="selbb(bb.Value,bb.DisplayName)">{{bb.DisplayName}}</a></li>';
    obj += ' </ul>  ';
    obj += '  </div><!-- /btn-group -->  ';
    obj += '  <div class="btn-group"> ';
    obj += ' <button id="btnselnj" ng-click="toggledrop(' + "'select_nj'" + ')" class="btn btn-warning dropdown-toggle" aria-expanded="false" aria-haspopup="true" type="button" data-toggle="dropdown" ng-init="selnjvalue=\'选择年级\'">{{selnjvalue}} <span class="caret"></span></button>';
    obj += ' <ul class="dropdown-menu" id="select_nj">  ';
    obj += '<li ng-repeat="nj in njs"><a href="#" val="nj.Value" ng-click="selnj(nj.Value,nj.DisplayName)">{{nj.DisplayName}}</a></li>';
    obj += ' </ul>  ';
    obj += '  </div><!-- /btn-group -->  ';
    obj += '  <div class="btn-group"> ';
    obj += ' <button  id="btnselzj" ng-click="toggledrop(' + "'select_zj'" + ')" class="btn btn-danger dropdown-toggle" aria-expanded="false" aria-haspopup="true" type="button" data-toggle="dropdown" ng-init="selzjvalue=\'选择知识章节\'">{{selzjvalue}} <span class="caret"></span></button>  ';
    obj += ' <div id="select_zj" class="dropdown-menu customtree" style="width:250px;height:300px;overflow:auto;"><ul>  ';
    obj += '<li ng-repeat="zj in zjs"><tree-View></tree-View></li>';
    obj += ' </ul></div>';
    obj += '  </div>';
    obj += '</div>  ';
    return {
        restrice: 'AE',
        replace: true,
        template: obj,
        link: function(scope, elem, attrs) {
            scope.xds = [{ Name: "小学", value: "7813E19A-1BF5-4219-B013-168A9297598F" }, { Name: "初中", value: "18910BD9-E745-498B-9702-1962D7955283" }, { Name: "高中", value: "0995408E-D28E-41B5-8BB6-351262CCBA05" }];
            scope.toggledrop = function (id) {
                $("#" + id).toggle();                
            };
	    //根据学段选择学科
            scope.selxd = function(value, name) {
                scope.selxdvalue = name;
                scope.selknowledge = value;
                scope.selknowledgePath = name;
                scope.getxk(value);
                $("#select_xd").hide();
            };
            //scope.xks2 = [{ Name: "小学语文", value: "1", parent: "7813E19A-1BF5-4219-B013-168A9297598F" }, { Name: "初中语文", value: "2", parent: "18910BD9-E745-498B-9702-1962D7955283" }, { Name: "高中语文", value: "3", parent: "0995408E-D28E-41B5-8BB6-351262CCBA05" }];
            scope.getxk = function(value) {
                $http.get('http://jp.taedu.gov.cn:84/Repository.Api/api/CacheGetJson/GetSubject?schoolstageid=' + value)
                    .success(function(response) {
                        var data = eval('(' + response + ')');
                        console.log(data);
                        scope.xks = [];
                        angular.forEach(data, function(item, index) {
                            scope.xks.push(item);
                        });
                    });
            };
            //end            
            //根据学科选择版本
            scope.selxk = function(value, name) {
                scope.selxkvalue = name;
                scope.selknowledge = value;
                scope.selknowledgePath += "/"+name;
                scope.getbb(value);
                $("#select_xk").hide();
            };
            //scope.bbs2 = [{ Name: "人教版", value: "1", parent: "1" }, { Name: "鲁教版", value: "2", parent: "2" }, { Name: "沪教版", value: "3", parent: "3" }];
            //scope.bbs = [];
            scope.getbb = function(value) {
                $http.get('http://jp.taedu.gov.cn:84/Repository.Api/api/CacheGetJson/GetVersion?SubjectId=' + value)
                    .success(function(response) {
                        var data = eval('(' + response + ')');
                        console.log(data);
                        scope.bbs = [];
                        angular.forEach(data, function(item, index) {
                            scope.bbs.push(item);
                        });
                    });
                //angular.forEach(scope.bbs2, function (item, index) {
                //    console.log(item.parent);
                //    console.log(value);
                //    if (item.parent == value) {
                //        scope.bbs.push(item);
                //    } else {
                //        scope.bbs.splice(index, 1);
                //    }
                //});
            };
            //end            
            //根据版本选择年级
            scope.selbb = function(value, name) {
                scope.selbbvalue = name;
                scope.selknowledge = value;
                scope.selknowledgePath += "/" + name;
                scope.getnj(value);
                $("#select_bb").hide();
            };
            //scope.njs2 = [{ Name: "小一年级", value: "1", parent: "1" }, { Name: "初一年级", value: "2", parent: "2" }, { Name: "高一年级", value: "3", parent: "3" }];
            //scope.njs = [];
            scope.getnj = function(value) {
                $http.get('http://jp.taedu.gov.cn:84/Repository.Api/api/CacheGetJson/GetGrade?VersionId=' + value)
                    .success(function(response) {
                        var data = eval('(' + response + ')');
                        console.log(data);
                        scope.njs = [];
                        angular.forEach(data, function(item, index) {
                            scope.njs.push(item);
                        });
                    });
                //angular.forEach(scope.njs2, function (item, index) {
                //    console.log(item.parent);
                //    console.log(value);
                //    if (item.parent == value) {
                //        scope.njs.push(item);
                //    } else {
                //        scope.njs.splice(index, 1);
                //    }
                //});
            };
            //end            
            //根据年级选择章节
            scope.selnj = function(value, name) {
                scope.selnjvalue = name;
                scope.selknowledgePath += "/" + name;
                scope.selknowledge = value;
                scope.getzj(value);
                $("#select_nj").hide();
            };
            //scope.zjs2 = [{ Name: "第一章", value: "1", parent: "1" }, { Name: "第二章", value: "2", parent: "2" }, { Name: "第三章", value: "3", parent: "3" }];
            //scope.zjs = [];
            scope.getzj = function(value) {
                $http.get('http://jp.taedu.gov.cn:84/Repository.Api/api/CacheGetJson/GetTextBook?GradeId=' + value)
                    .success(function(response) {
                        var data = eval('(' + response + ')');
                        console.log(data);
                        scope.zjs = data;
                        //angular.forEach(data, function (item, index) {
                        //    scope.zjs.push(item);
                        //});
                    });
                //angular.forEach(scope.zjs2, function (item, index) {
                //    console.log(item.parent);
                //    console.log(value);
                //    if (item.parent == value) {
                //        scope.zjs.push(item);
                //    } else {
                //        scope.zjs.splice(index, 1);
                //    }
                //});
            };
            //end
            //选择章节
            scope.selzj = function(value, name) {
                scope.selzjvalue = name;
                scope.selknowledgePath += "/" + name;
                scope.selknowledge = value;
                $("#select_zj").hide();
            };
            scope.$watch('selxdvalue', function() {
                scope.selxkvalue = "选择学科";
                scope.selbbvalue = "选择版本";
                scope.selnjvalue = "选择年级";
                scope.selzjvalue = "选择章节";
                
            });
            scope.$watch('selxkvalue', function () {
                scope.selbbvalue = "选择版本";
                scope.selnjvalue = "选择年级";
                scope.selzjvalue = "选择章节";
            });
            scope.$watch('selbbvalue', function () {
                scope.selnjvalue = "选择年级";
                scope.selzjvalue = "选择章节";
            });
            scope.$watch('selnjvalue', function () {
                scope.selzjvalue = "选择章节";
            });
        }
    };
});
