$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var taskid = $("#TaskID").val();
    $.ajax({
        "dataType": 'json',
        "type": "GET",
        "url": url_gettasktarget,
        "data": { "TaskID": taskid },
        "success": function (response) {
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "TaskTargets" },
    { data: "TaskType" },
    { data: "TargetsNum" },
    { data: "PKID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
{
        targets: [3],
        data: "PKID",
        render: function (data, type, row) {
            return "<a href='usermanage' title='任务条件' onclick=\"javascript:editTabRow('" + data + "');\">[任务条件]</a>&nbsp;" +
                "<a href='#' title='任务范围'  onclick=\"javascript:recoveryTabRow('" + data + "');\">[任务范围]</a>&nbsp;" +
                "<a href='#' title='任务目标'  onclick=\"deleteTabRow('" + data + "');\">[任务目标]</a>&nbsp;" +
                "<a href='#' title='任务奖励'  onclick=\"deleteTabRow('" + data + "');\">[任务奖励]</a>";
        }
    }];

var userdataTable = $("#table_target").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchuser").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_target").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});
function editTabRow(data) {

}
function deleteTabRow(data) {
    $("body").append("<div id='dialog-taskcondition'  class='modal fade in ' title='任务条件'></div>")
    $("#dialog-taskcondition").load("taskmanage?TaskID=" + data);
    $("#dialog-taskcondition").dialog({
        autoOpen: true,
        modal: true,
        width: 900,
        height: 560,
        buttons: {
            "确认": function () {
                var skin = $("input:radio:checked").val();
                $.ajax({
                    url: "http://www.baidu.com",
                    data: { skintheme: skin, skincolor: "8" },
                    success: function (data) {
                        console.log(data);
                        if (data.Data.statuscode == "200") {
                            document.location.href = document.location.href;
                        }
                    }
                });
                $(this).dialog("close");
            },
            "取消": function () {
                $(this).dialog("close");
            }
        }

    });
}
function recoveryTabRow(data) {
    alert(data);
}