$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var dicttype = $("#search_dicttype").val();
    var dictname = $("#search_dictname").val();
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        "data": { "dicttype": dicttype, "dictname": dictname },
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "DictCode" },
    { data: "DictName" },
    { data: "DictType" },
    { data: "DictOrder" },
    { data: "IsState" },
    { data: "DictID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [   
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
        data: "DictID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editdictionary('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletedictionary('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editdictionary('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoverdictionary('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_dictionary").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchdictionary").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_dictionary").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});

function deletedictionary(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "dictid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_dictionary").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editdictionary(data) {
    RedircetUrl(operurl_edit + "?dictid=" + data);
}
function rescoverdictionary(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "dictid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_dictionary").dataTable($.dataTablesSettings).fnDraw(false);
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
        data: { "dictid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("设置成功！")
                $("#table_dictionary").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}