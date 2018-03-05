toastr.options = {
    "positionClass": "toast-top-center"
};
var removetheme = function (skinid, skinstatus) {//note: the function name is important
    delconfirm("确认要移除此项吗？", function () {
        $.get('/uchome/sysManage/MoveSkin?pkid=' + skinid + '&skinstatus=' + skinstatus, function (response) {
            if (response.Data.statuscode === "200") {
                tipfailmsg("禁用成功")
                document.location.reload();
            }
            else {
               tipfailmsg("禁用失败")
            }
        });
    }, null);
}
var getskin = function (skin) {
    $.get('/uchome/sysManage/GetSysSkin?pkid=' + skinid, function (response) {
        if (response.Data.statuscode === "200") {
            tipfailmsg("获取成功")
            document.location.reload();
        }
        else {
            tipfailmsg("获取失败")
        }
    });
}