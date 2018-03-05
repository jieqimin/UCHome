$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        //"data": { "XM": xm, "UserName": username },
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "CalName" },
    { data: "CalStartDate" },
    { data: "CalEndDate" },
    { data: "IsCurrent" },
    { data: "IsState" },
    { data: "CalID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [1],
        data: "CalStartDate",
        render: function (data, type, row) {
            var startdate = new Date(parseInt(data.replace(/\D/igm, "")));
            return startdate.toLocaleDateString();
        }
    },
    {
        targets: [2],
        data: "CalEndDate",
        render: function (data, type, row) {
            var enddate = new Date(parseInt(data.replace(/\D/igm, "")));
            return enddate.toLocaleDateString();
        }
    },
    {
        targets: [3],
        data: "IsCurrent",
        render: function (data, type, row) {
            if (data == "1")
                return "<span class='label label-success'>当前学期</span>";
            return "<a  title='设为当前学期' class='nounderline' onclick=\"javascript:setcurrent('" + row.CalID + "');\">设为当前学期</a>";
        }
    },
    {
        targets: [4],
        data: "State",
        render: function (data, type, row) {
            if (data == 1)
                return "<span class='label label-success'>正常</span>";
            return "<span class='label label-danger'>已禁用</span>"
        }
    }, {
        targets: [5],
        data: "CalID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editsicalendar('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletesicalendar('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editsicalendar('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoversicalendar('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_sicalendars").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
//$("#btn-searchuser").click(function () {
//    userdataTable.fnDestroy(false);
//    userdataTable = $("#table_users").dataTable($.dataTablesSettings);
//    //搜索后跳转到第一页
//    userdataTable.fnPageChange(0);
//});

function deletesicalendar(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "CalID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_sicalendars").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editsicalendar(data) {
    RedircetUrl(operurl_edit + "?calid=" + data);
}
function rescoversicalendar(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "CalID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_sicalendars").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("恢复失败！")
            }
        }
    })
}
function setcurrent(data) {
    $.ajax({
        type: "POST",
        url: operurl_set,
        data: { "CalID": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("设置成功！")
                $("#table_sicalendars").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}