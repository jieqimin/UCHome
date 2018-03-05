$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    $.ajax({
        "dataType": 'json',
        "type": "GET",
        "url": getalltaskurl,

        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "TaskName" },
    { data: "TaskUrl" },
    { data: "TaskStatus" },
    { data: "PKID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [1],
        data: "TaskUrl",
        render: function (data, type, row) {
            return "<a href='" + data + "'>任务链接定向</a>"
        }
    },
    {
        targets: [2],
        data: "TaskStatus",
        render: function (data, type, row) {
            if (data == 1)
                return "正常";
            return "已禁用"
        }
    }, {
        targets: [3],
        data: "PKID",
        render: function (data, type, row) {
            return "<a href='#' title='任务条件' onclick=\"showtaskcondition('" + data + "','" + url_taskcondition +"','任务条件');\">[任务条件]</a>&nbsp;" +
                "<a href='#' title='任务范围'  onclick=\"showtaskcondition('" + data + "','" + url_tasktrigger +"','任务范围');\">[任务范围]</a>&nbsp;" +
                "<a href='#' title='任务目标'  onclick=\"showtaskcondition('" + data + "','" + url_tasktarget +"','任务目标');\">[任务目标]</a>&nbsp;" +
                "<a href='#' title='任务奖励'  onclick=\"showtaskcondition('" + data + "','" + url_taskreward +"','任务奖励');\">[任务奖励]</a>";
        }
    }];

var userdataTable = $("#table_task").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchuser").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_task").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});
function showtaskcondition(data,loadurl,title) {
    $("body").append("<div id='dialog-taskcondition'  class='modal fade in ' title='"+title+"'></div>")
    $("#dialog-taskcondition").load(loadurl+"?TaskID=" + data);
    $("#dialog-taskcondition").dialog({
        autoOpen: true,
        modal: true,
        width: 900,
        height: 560,
        buttons: {
            "确认": function () {
                $(this).dialog("close");
                $("#dialog-taskcondition").remove();
            },
            "取消": function () {
                $(this).dialog("close");
                $("#dialog-taskcondition").remove();
            }
        }

    });
}
