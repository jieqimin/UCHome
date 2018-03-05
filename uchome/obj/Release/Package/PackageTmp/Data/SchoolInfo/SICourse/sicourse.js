var coursetypes;
$.get(coursetypegeturl, function (data) {
    coursetypes = data;
})
$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var coursetype = $("#search_coursetype").val();
    var dictname = $("#search_dictname").val();
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        "data": { "coursetype": coursetype },
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "CourseName" },
    { data: "CourseID_DMName" },
    { data: "CourseType_DictCode" },
    { data: "CourseNature_DMName" },
    { data: "IsState" },
    { data: "CourseID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [2],
        data: "CourseType_DictCode",
        render: function (data, type, row) {
            var coursetype = $.grep(coursetypes, function (item) {
                    return item.DictCode==data;
            })
            if (coursetype.length == 1)
                return coursetype[0].DictName;
            else
                return "未找到";
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
        data: "CourseID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editsicourse('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletesicourse('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editsicourse('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoversicourse('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_sicourse").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchsicourse").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_sicourse").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});

function deletesicourse(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "courseid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_sicourse").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editsicourse(data) {
    RedircetUrl(operurl_edit + "?courseid=" + data);
}
function rescoversicourse(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "courseid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_sicourse").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("恢复失败！")
            }
        }
    })
}