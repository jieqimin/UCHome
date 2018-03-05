function redirecturl(src) {
    if (src != null && src != "") {
        $("#id-app-content").load(src);
    }
    else {
        $("#id-app-content").html("<div class='form-position pagew'>抱歉，暂未找到可显示信息！</div>");
    }
}
function resetNavMenuClass(index) {
    var obj = $("#id-app-menu ul li");
    obj.removeClass("on");
    obj.eq(index).addClass("on");
}
function RedircetUrl(url) {
    $("#id-app-content").load(url);
}
function OpenAppUrl(url) {
    loaddata($("#body_content"));
    $("#body_content").load(url);
}


