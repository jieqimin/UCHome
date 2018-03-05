tempApp.controller("ValidateDemo", ngTreeCtrlFn);
function ngTreeCtrlFn($scope) {
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

