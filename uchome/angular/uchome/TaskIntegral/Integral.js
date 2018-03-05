tempApp.controller("Integral", ['$scope', '$http', function ($scope, $http) {
    $scope.Data = [];
    $scope.model = {};
    $scope.RefreshData= function() {
        $http.get("/uchome/TaskIntegral/GetAllData").success(function (response) {
            if (response.flag) {
                console.info(response.list);
                $scope.Data=response.list;
            } else {
                console.info(response.message);
            }
        });
    }
    $scope.Remove = function (item) {
        item.Scores = item.Scores + "";
        $http.post("/uchome/TaskIntegral/DeleteIntegral", { pkID: item.pkID }).success(function (response) {
            if (response.flag) {
                $scope.RefreshData();
            } else {
                console.info(response.message);
            }
        });
    }
    $scope.Add = function (valid) {
        if (valid) {
            var item = $scope.model;
            item.Scores = item.Scores + "";
            $http.post("/uchome/TaskIntegral/AddIntegral", item).success(function (response) {
                if (response.flag) {
                    $scope.RefreshData();
                    $scope.model = { LevelIcon: '' }
                } else {
                    console.info(response.message);
                }
            });
        }
        
    }
    $scope.RefreshData();
}]);