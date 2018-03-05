$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var xm = $("#xm").val().trim();
    var username = $("#username").val().trim();
    $.ajax({
        "dataType": 'json',
        "type": "POST",
        "url": initlisturl,
        "data": { "XM": xm, "UserName": username },
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "XM" },
    { data: "UserName" },
    { data: "UserType" },
    { data: "UserType" },
    { data: "State" },
    { data: "PKID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [2],
        data: "UserType",
        render: function (data, type, row) {
            if (data == "s")
                return "学生";
            return "教师";
        }
    },
    {
        targets: [3],
        data: "UserType",
        render: function (data, type, row) {
            if (data == "s")
                return row.XJH;
            return row.SFZH
        }
    },
    {
        targets: [4],
        data: "State",
        render: function (data, type, row) {
            if (data == 1)
                return "正常";
            return "已禁用"
        }
    }, {
        targets: [5],
        data: "PKID",
        render: function (data, type, row) {
            return "<a title='重置密码' class='glyphicon glyphicon-edit nounderline' onclick=\"javascript:editTabRow('" + data + "');\"></a>&nbsp;" +
                "<a title='恢复' class='glyphicon glyphicon-floppy-saved nounderline' onclick=\"javascript:recoveryTabRow('" + data + "');\"></a>&nbsp;" +
                "<a title='禁用' class='glyphicon glyphicon-trash nounderline' onclick=\"deleteTabRow('" + data + "');\"></a>";
        }
    }];

var userdataTable = $("#table_users").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchuser").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_users").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});
function editTabRow(data) {
    $.ajax({
        url: operurl_edit,
        data: { "PKID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("重置成功，重置密码为123456！")
                $("#table_users").dataTable($.dataTablesSettings).fnDraw(false);
            }
        }
    })
}
function deleteTabRow(data) {
    $.ajax({
        url: operurl_delete,
        data: { "PKID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功，如果操作错误可以通过用户回收进行回复！")
                $("#table_users").dataTable($.dataTablesSettings).fnDraw(false);
            }
        }
    })

}
function recoveryTabRow(data) {
    $.ajax({
        url: operurl_recovery,
        data: { "PKID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_users").dataTable($.dataTablesSettings).fnDraw(false);
            }
        }
    })

}