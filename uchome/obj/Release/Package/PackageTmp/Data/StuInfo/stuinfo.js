var curyear;
$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var stu = $("#search_stu").val();
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        "data": { stu: stu },
        "success": function (response) {
            $("#records").text(response.recordcount);
            curyear = response.curyt;
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "StuID_BJID" },
    { data: "classname" },
    { data: "StuName" },
    { data: "StuCode" },
    { data: "IsState" },
    { data: "StuID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [0],
        data: "StuID_BJID",
        render: function (data, type, row) {
            var classstage = row.classstage;
            var createyear = row.classcreateyear;
            returngrade(classstage, curyear, createyear);
        }
    },
    {
        targets: [4],
        data: "IsState",
        render: function (data, type, row) {
            if (data == 1)
                return "<span class='label label-success'>正常</span>";
            return "<span class='label label-danger'>已禁用</span>"
        }
    }, {
        targets: [5],
        data: "StuID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editstuinfo('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletestuinfo('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editstuinfo('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoverstuinfo('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_stuinfo").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchstu").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_stuinfo").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});

function deletestuinfo(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "stuid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_stuinfo").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editstuinfo(data) {
    RedircetUrl(operurl_edit + "?stuid=" + data);
}
function rescoverstuinfo(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "stuid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_stuinfo").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("恢复失败！")
            }
        }
    })
}