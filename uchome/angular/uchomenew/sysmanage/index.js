angular.module("sysmanage", []).controller("sysmanage", function($scope) {    
});
$("#sysman li").each(function (index, item) {
    alert();
    $(this).on("click", function () {
        alert($(this).html());
        $(this).addClass("active");
    });
})