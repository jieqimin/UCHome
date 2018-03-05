tempApp.controller("index", PageViewInit);
function PageViewInit($scope) {
    $scope.name = "enter";
    $scope.submitForm = function (isValid) {
        if (!isValid) {
            alert('验证失败');
        }
        else {
            alert('fail');
        }
    };
};

