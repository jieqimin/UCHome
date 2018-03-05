var groundtypes;
$.get(groundtypegeturl, function (data) {
    groundtypes = data;
})
$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "GroundType_DictCode" },
    { data: "GroundCode" },
    { data: "GroundName" },
    { data: "IsState" },
    { data: "GroundID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [0],
        data: "GroundType_DictCode",
        render: function (data, type, row) {
            var groundtype = $.grep(groundtypes, function (item) {
                return item.DictCode == data;
            })
            if (groundtype.length == 1)
                return groundtype[0].DictName;
            else
                return "未找到";
        }
    }, 
    {
        targets: [3],
        data: "IsState",
        render: function (data, type, row) {
            if (data == 1)
                return "<span class='label label-success'>正常</span>";
            return "<span class='label label-danger'>已禁用</span>"
        }
    }, {
        targets: [4],
        data: "GroundID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editsiground('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletesiground('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editsiground('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoversiground('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_siground").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchsiground").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_siground").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});

function deletesiground(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "groundid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_siground").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editsiground(data) {
    RedircetUrl(operurl_edit + "?groundid=" + data);
}
function rescoversiground(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "groundid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_siground").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("恢复失败！")
            }
        }
    })
}