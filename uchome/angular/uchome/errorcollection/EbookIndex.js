tempApp.filter('htmlContent',['$sce', function($sce) {
    return function(input) {
        return $sce.trustAsHtml(input);
    }
}]);
tempApp.controller("EbookIndex", ['$scope', '$http', function ($scope, $http) {
    $scope.QuestionType = [];
    
    $scope.TypeList = [];
    $scope.ReasonList = [];
    $scope.subjectLists = [];
    $scope.subjectId = "";
    $scope.subjectName = "";
    $scope.versionList = [];
    $scope.versionId = "";
    $scope.versionName = "";
    $scope.gradeList = [];
    $scope.gradeId = "";
    $scope.gradeName = "";
    $scope.textList = [];
    $scope.textName = "";
    $scope.textId = "";
    $scope.type = "";
    $http.get("http://localhost/uchome/errorcollection/GetEbookquestiontype").success(function (response) {
        response = eval("(" + response + ")");
        if (response.Result.length > 0) {
            $.each(response.Result, function(idx, item) {
                $scope.QuestionType[item.Id] = item.Name;
            });
        }
    });
    $http.get("http://localhost/uchome/errorcollection/GetStudentErrorSubject").success(function (response) {
        response = eval("(" + response + ")");
        $scope.subjectLists = response.Result;
    });
    $http.get('http://localhost/uchome/errorcollection/GetQuestionType').success(function (response) {
        //response = eval("(" + response + ")");
        $scope.TypeList = response;
    });
    $http.get('http://localhost/uchome/errorcollection/GetErrorReason').success(function (response) {
        //response = eval("(" + response + ")");
        $scope.ReasonList = response;
    });
    $scope.QuestionHardLevel = ["容易", "较易", "一般", "较难", "最难"];
    $scope.toggleDrop= function(id) {
        $("#" + id).toggle();
    }
    $scope.GetVersion= function(subject,name) {
        $scope.subjectId = subject;
        $scope.subjectName = name;
        $http.get("http://localhost/uchome/errorcollection/GetStudentErrorVersion?subject=" + $scope.subjectId).success(function (response) {
            response = eval("(" + response + ")");
            $scope.versionList = response.Result;
        });
        $scope.toggleDrop("subjects");
    }
    $scope.GetGrade = function (version,name) {
        $scope.versionId = version;
        $scope.versionName = name;
        $http.get("http://localhost/uchome/errorcollection/GetStudentErrorGrade?subject=" + $scope.subjectId + "&version=" + $scope.versionId).success(function (response) {
            response = eval("(" + response + ")");
           $scope.gradeList = response.Result;
        });
        $scope.toggleDrop("versions");
    }
    $scope.GetTextBook = function (value, name) {
        $scope.gradeId = value;
        $scope.gradeName = name;
        $http.get('http://jp.taedu.gov.cn:84/Repository.Api/api/CacheGetJson/GetTextBook?GradeId=' + value)
            .success(function(response) {
                var data = eval('(' + response + ')');
                $scope.textList = data;
            });
        $scope.toggleDrop("grades");
    }
    $scope.SelectText = function (value, name) {
        $scope.textName = name;
        $scope.textId = value;
        $scope.toggleDrop("texts");
    }
    $scope.SelectType= function(value, name) {
        $scope.type = value;
        $scope.toggleDrop("types");
        
    }
    $scope.showErrorModal= function(eitem) {
        $("#modal").modal('show');
        $scope.showModal = eitem;
    }
    $scope.CurrentPage = 1;
    $scope.GetEBookList = function () {
        var postData = {
            "ChooseType": null,
            "SubjectId": $scope.subjectId,
            "GradeId": $scope.gradeId,
            "TextbookId": "",
            "CatalogId": $scope.textId,
           
            "PageIndex": $scope.CurrentPage,
            "PageSize": 4
        };
        $http.post("http://localhost/uchome/errorcollection/GetStudentErrorCollect", postData).success(function (response) {
            response = eval("(" + response + ")");
            $scope.errorcollections = [];
            $scope.Ids = [];
            if (response.Result.Results.length > 0) {
                $.each(response.Result.Results, function(idx,item ) {
                    var errorItem = {};
                    $scope.Ids.push(item.ErrorId);
                    errorItem.QKnowledge = "";
                    if (item.Subject.Id != null) {
                        errorItem.QKnowledge += "/" + item.Subject.Name;
                        errorItem.QKID = item.Subject.Id;
                    }
                    if (item.Version.Id != null) {
                        errorItem.QKnowledge += "/" + item.Version.Name;
                        errorItem.QKID = item.Version.Id;
                    }
                    if (item.Grade.Id != null) {
                        errorItem.QKnowledge += "/" + item.Grade.Name;
                        errorItem.QKID = item.Grade.Id;
                    }
                    if (item.Book.Id != null) {
                        errorItem.QKnowledge += "/" + item.Book.Name;
                        errorItem.QKID = item.Book.Id;
                    }
                    errorItem.PKId = item.ErrorId;
                    errorItem.QName = item.Title;
                    errorItem.QDifficulty = item.HardType.Id;
                    errorItem.RightAnwer = $scope.Transfer(item.Answer, item.ChooseTypeId);
                    errorItem.WrongAnwer = $scope.Transfer(item.ErrorAnswer, item.ChooseTypeId);
                    errorItem.QuestionType = item.ChooseTypeId;
                    errorItem.EditDate = item.ErrorCreateTime;
                    errorItem.QMemo = item.Body.replace('src="/TaiXue', 'src="http://jxts.taedu.gov.cn:96/TaiXue');
                    errorItem.Status = 0;
                    $scope.errorcollections.push(errorItem);
                });
            }
            
            $("#page").bs_pagination({
                currentPage: $scope.CurrentPage,
                visiblePageLinks: 7,
                totalPages: response.Result.TotalPages, //根据实际数量/visiblePageLinks
                showGoToPage: false,
                showRowsPerPage: false,
                showRowsInfo: false,
                onChangePage: function (type, page) {
                    $scope.CurrentPage = page.currentPage;
                    $scope.GetEBookList();
                }
            });
            if ($scope.Ids.length > 0) {
                $scope.GetErrorStatus($scope.Ids);
            }
            
            
        });
    }

    $scope.GetErrorStatus= function(ids) {
        $http.post("/uchome/errorcollection/geterrorstatus", ids).success(function(response) {
            $.each($scope.errorcollections, function (idx2, item2) {
                $.each(response, function(idx, item) {
                    if (item == item2.PKId) {
                        item2.Status = 1;
                    }
                });
            });
        });
        //$scope.$apply();
    }

    $scope.Transfer = function (str, type) {
        str = str + "";
        if (type === 3) {
                return str === "1" ? '对' : '错';
        }
        else if (type === 2 || type === 1) {
            if (str.length > 1)
                str = str.replace('1', "A").replace('2', "B").replace('3', "C").replace('4', "D").replace('5', "E").replace('6', "F").replace('7', "G").replace('8', "H").replace('9', "I").split(',').join(" ");
            else {
                switch (str) {
                    case "1":
                        str = "A";
                        break;
                    case "2":
                        str = "B";
                        break;
                    case "3":
                        str = "C";
                        break;
                    case "4":
                        str = "D";
                        break;
                    case "5":
                        str = "E";
                        break;
                    case "6":
                        str = "F";
                        break;
                    case "7":
                        str = "G";
                        break;
                    case "8":
                        str = "H";
                        break;
                    case "9":
                        str = "I";
                        break;
                    default:
                        break;
                }
            }
        }
        return str;
    }
    $scope.GetEBookList();

    $scope.EditError = function (errorItem) {
        $scope.showModal = errorItem;
        $scope.showModal.Insert = true;
        $("#modal").modal('show');
    }
    $scope.SaveModal = function (obj) {
        $(obj).attr("disabled", "disabled");
        var typelist = [];
        $("#questionType input[type=checkbox]:checked").each(function(idx, item) {
            typelist.push(item.value);
        });
        var errorlist = [];
        $("#questionError input[type=checkbox]:checked").each(function(idx, item) {
            errorlist.push(item.value);
        });
        var postData = {
            "types": typelist,
            "reasons": errorlist,
            "model":$scope.showModal
        };
        $http.post("http://localhost/uchome/errorcollection/saveErrorFromTaixue", postData).success(function(response) {
            if (response.flag) {
                alert("保存成功");
                $(obj).removeAttr("disabled");
                $("#modal").modal('hide');
                $.each($scope.errorcollections, function(idx, item) {
                    if (item.PKId == $scope.showModal.PKId) {
                        item.Status = 1;
                    }
                });
                $scope.$apply();
            } else {
                alert(response.error);
                $(obj).removeAttr("disabled");
                $("#modal").modal('hide');
            }
        });
    }
}]);