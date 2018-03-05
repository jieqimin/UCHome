$(function () {
    $("#ulGroup .a-department-item").click(function () {
        var datatype = $(this).attr("data-type");
        getData(this, datatype);
    });

    $("#ulGroup .a-department-item").dblclick(function () {
        selectAll();
    });

    $(".query").keyup(function () {

        var selRole = $("#selRole").val();
        var selArea = $("#selArea").val();
        var query = $.trim($(".query").val());

        if (query != "" && query != null) {
            $("#ulGroup").hide();

            $.getJSON(queryUrl, { "selRole": selRole, "selArea": selArea, "query": query }, function (data) {
                $("#ulUserList a").remove();
                $.each(data, function (i, item) {
                    console.log(item);
                    if (!isExit(item.userId)) {
                        if (!isSelected(item.userId)) {
                            var str = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.userId + " displayname=" + item.XM+"><span class='glyphicon glyphicon-arrow-right' style='left: 80px; color: #ddd;'></span>" + item.XM + "</a>";
                            $("#ulUserList").append(str);
                        }
                        else {
                            var str = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.userId + " displayname=" + item.XM +" style=\"color:green\"><span class='glyphicon glyphicon-ok' style='left: 80px; color: green;'></span>" + item.XM + "</a>";
                            $("#ulUserList").append(str);
                        }
                    }
                    return true;
                });
            });
        } else {
            $("#ulGroup").show();

            var obj = $("#ulGroup li span").eq(0);
            var first_datatype = obj.parent().attr("data-type");
            if (typeof (first_datatype) == "undefined") {
                var tmpObj = obj.siblings().children(":first");
                first_datatype = tmpObj.attr("data-type");
                var newObj = tmpObj.children(":first");
                getData(newObj, first_datatype);
            } else {
                getData(obj, first_datatype);
            }
        }
    });

});

function getData(obj, datatype) {
    $("#ulGroup .a-department-item").each(function () {
        $(this).removeClass("active");
    });
    $(obj).addClass("active");

    var selRole = $("#selRole").val();
    if (datatype == "su_friend") {
        getAttentionUserList(attentionUrl);
        showSelAll(obj);
    } else if (datatype == "su_student") {
        var bjid = $(obj).attr("id");
        getStudentList(studentUrl, bjid, selRole);
        showSelAll(obj);
    } else if (datatype == "su_stuparent") {
        var bjid = $(obj).parent().attr("id");
        getStuParentList(stuParentUrl, bjid, selRole);
        showSelAll(obj);
    } else if (datatype == "su_department") {
        var deptid = $(obj).attr("id");
        GetDeptUserList(teadeptUrl, deptid, selRole);
        showSelAll(obj);
    }
}

function isExit(userid) {
    var flag = false;
    $("#ulUserList li").each(function () {
        var id = $(this).attr("id");
        if (id == userid) {
            flag = true;
            return false;//跳出循环
        }
    });
    return flag;
}
function isSelected(userid) {
    var flag = false;
    $("#ulSelectedUser li .spname").each(function () {
        var id = $(this).attr("id");
        if (id == userid) {
            flag = true;
            return false;//跳出循环
        }
    });
    return flag;
}
function getAttentionUserList(url) {
    $.getJSON(url, function (data) {
        $("#ulUserList a").remove();
        $.each(data, function (i, item) {
            if (!isExit(item.AttenUser)) {
                if (!isSelected(item.AttenUser)) {
                    var str1 = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.AttenUser + " displayname=" + item.AttenName + "><span class='glyphicon glyphicon-arrow-right' style='left: 80px; color: #ddd;'></span>" + item.AttenName + "</a>";
                    $("#ulUserList").append(str1);
                }
                else {
                    var str2 = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.AttenUser + " displayname=" + item.AttenName + " style=\"color:green\"><span class='glyphicon glyphicon-ok' style='left: 80px; color: green;'></span>" + item.AttenName + "</a>";
                    $("#ulUserList").append(str2);
                }
            }
        });
    });
}

function getStudentList(url, bjid, selRole) {
    $.getJSON(url, { "bjid": bjid, "selRole": selRole }, function (data) {
        $("#ulUserList a").remove();
        $.each(data, function (i, item) {
            if (!isExit(item.XSID)) {
                if (!isSelected(item.XSID)) {
                    var str1 = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.XSID + " displayname=" + item.XM + "><span class='glyphicon glyphicon-arrow-right' style='left: 80px; color: #ddd;'></span>" + item.XM + "</a>";
                    $("#ulUserList").append(str1);
                }
                else {
                    var str2 = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.XSID + " displayname=" + item.XM + " style=\"color:green\"><span class='glyphicon glyphicon-ok' style='left: 80px; color: green;'></span>" + item.XM + "</a>";
                    $("#ulUserList").append(str2);
                }
            }
        });
    });
}

function getStuParentList(url, bjid, selRole) {
    $.getJSON(url, { "bjid": bjid, "selRole": selRole }, function (data) {
        $("#ulUserList li").remove();
        $.each(data, function (i, item) {
            if (!isExit(item.JZID) && !isSelected(item.JZID)) {
                var str = "<li onclick=\"select(this)\" id=" + item.JZID + ">" + item.XM + "</li>";
                $("#ulUserList").append(str);
            }
        });
    });
}
function GetDeptUserList(url, deptid) {
    console.log(url);
    console.log(deptid);
    $.getJSON(url, { "deptid": deptid, }, function (data) {
        $("#ulUserList a").remove();
        $.each(data, function (i, item) {
            //去重
            if (!isExit(item.TeacherId)) {
                if (!isSelected(item.TeacherId)) {
                    var str1 = "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.TeacherId + " displayname=" + item.DisplayName + "><span class='glyphicon glyphicon-arrow-right' style='left: 80px; color: #ddd;'></span>" + item.DisplayName + "</a>";
                    $("#ulUserList").append(str1);
                }
                else {
                    var str2= "<a href='#' class='list-group-item' onclick=\"select(this)\" id=" + item.TeacherId + " displayname=" + item.DisplayName + " style=\"color:green\"><span class='glyphicon glyphicon-ok' style='left: 80px; color: green;'></span>" + item.DisplayName + "</a>";
                    $("#ulUserList").append(str2);
                }
            }
        });
    });
}
function dotoggle(obj) {
    $(".selall").hide();
    $("#" + obj).toggle();
}

function showSelAll(obj) {
    var seltype = $("#selType").val();
    if (seltype != "single") {
        $(".selall").hide();
        $(obj).siblings().show();
    }
}

function select(o) {
    var userid = $(o).attr("id");
    var uname = $(o).text();

    var seltype = $("#selType").val();
    var obj = $("#ulSelectedUser li .spname");
    if (seltype == "single" && obj.length > 0) {
        alert("只能选择一个人员！");
    } else {
        if (!isSelected(userid)) {
            var str = "<li onclick='clearData(this)' class='list-group-item-info'><span class='spname' id='" + userid + "'>" + uname + "</span>" +
                "<span class='spbtn'>" +
                "<button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
                "<span aria-hidden='true' class='spclose' >×</span>" +
                "</button></span></li>";

            $("#ulSelectedUser").append(str);
            $(o).css("color", "green").find("span").removeClass("glyphicon-arrow-right").addClass("glyphicon-ok").css("color", "green");
        }
    }
}

function clearData(o) {
    $(o).remove();
    var selid = $(o).find("span").attr("id");
    $("#ulUserList #" + selid).css("color", "#404040").find("span").removeClass("glyphicon-ok").addClass("glyphicon-arrow-right").css("color", "#ddd");
}

function selectAll() {
    var seltype = $("#selType").val();
    if (seltype != "single") {
        $("#ulUserList a").each(function (index, item) {
            var userid = $(this).attr("id");
            var uname = $(this).attr("displayname");
            var issel = isSelected(userid);
            if (!issel) {
                var str = "<li onclick='clearData(this)' class='list-group-item-info'><span class='spname' id='" + userid + "'>" + uname + "</span>" +
                    "<span class='spbtn'>" +
                    "<button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
                    "<span aria-hidden='true' class='spclose' >×</span>" +
                    "</button></span></li>";

                $("#ulSelectedUser").append(str);
                $(this).css("color", "green").find("span").removeClass("glyphicon-arrow-right").addClass("glyphicon-ok").css("color", "green");
            }
        });
    }
}