$(function () {
    $("#ulGroup li").click(function () {
        //$(this).siblings().removeClass("actgroup");
        $("#ulGroup li").each(function () {
            $(this).removeClass("actgroup");
        });
        $(this).addClass("actgroup");

        var datatype = $(this).attr("data-type");
        if (datatype == "1") {
            getAttentionUserList(attentionUrl);
        } else if (datatype == "2") {
            getColleagueList(colleagueUrl);
        } else if (datatype == "3") {
            var bjid = $(this).attr("id");
            getStudentList(studentUrl, bjid);
        }

    });

});

function getAttentionUserList(url) {
    $("#ulUserList li").remove();

    $.getJSON(url, function (data) {
        $.each(data, function (i, item) {
            var str = "<li onclick=\"select(this)\" id=" + item.AttenUser + ">" + item.AttenName + "</li>";
            $("#ulUserList").append(str);
        });
    });
}

function getColleagueList(url) {
    $("#ulUserList li").remove();

    $.getJSON(url, function (data) {
        $.each(data, function (i, item) {
            var str = "<li onclick=\"select(this)\" id=" + item.jsid + ">" + item.jsmc + "</li>";
            $("#ulUserList").append(str);
        });
    });
}

function getStudentList(url, bjid) {
    $("#ulUserList li").remove();

    $.getJSON(url, { "bjid": bjid }, function (data) {
        $.each(data, function (i, item) {
            var str = "<li onclick=\"select(this)\" id=" + item.JZID + ">" + item.XM + "家长</li>";
            $("#ulUserList").append(str);
        });
    });
}

function dotoggle() {
    $(".listbox-item").toggle();
}

function select(o) {
    var userid = $(o).attr("id");
    var uname = $(o).text();

    var len = $("#ulSelectedUser li .spname").length;
    if (len == 0) {
        var str = "<li><span class='spname' id='" + userid + "'>" + uname + "</span>" +
                  "<span class='spbtn'>" +
                  "<button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
                  "<span aria-hidden='true' class='spclose' onclick='clearData(this)'>×</span>" +
                  "</button></span></li>";

        $("#ulSelectedUser").append(str);

        $(o).remove();
    } else {
        alert("只能选择一个人员！");
    }
}

function clearData(o) {
    var obj = $(o).parent().parent().siblings();
    var userid = $(obj).attr("id");
    var uname = $(obj).text();
    var str = "<li onclick=\"select(this)\" id=" + userid + ">" + uname + "</li>";
    $("#ulUserList").append(str);

    $(o).parent().parent().parent().remove();
}
