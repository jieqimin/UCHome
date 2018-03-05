var curyear;
$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var searchtype = $("#stusearchtype").val();
    //模糊搜索
    if (searchtype == "2") {
        var stuname = $("#advsearch_stuname").val();
        var stucode = $("#advsearch_stucode").val();
        var stunumber = $("#advsearch_stunumber").val();
        var stuidentity = $("#advsearch_stuidentity").val();
        var grade = $("#search_grade").val();
        var classid = $("#search_class").val();
        $.ajax({
            "dataType": 'json',
            "type": "Get",
            "url": advsearchstuurl,
            "data": { grade: grade, classid: classid,stuname:stuname,stucode:stucode,stunumber:stunumber,stuidentity:stuidentity },
            "success": function (response) {
                console.log(response);
                $("#records").text(response.recordcount);
                curyear = response.curyt;
                fnCallback(response);
            }
        });
    }
    else {
        var stu = $("#search_stu").val();
        $.ajax({
            "dataType": 'json',
            "type": "Get",
            "url": initlisturl,
            "data": { stu: stu },
            "success": function (response) {
                console.log(response);
                $("#records").text(response.recordcount);
                curyear = response.curyt;
                fnCallback(response);
            }
        });
    }
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
            return returngrade(classstage, curyear, createyear);
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
                return "<a  title='详情' class='nounderline' onclick=\"querystuinfo('" + data + "');\">[详情]</a>&nbsp;" +
                    "<a  title='编辑' class='nounderline' onclick=\"editstuinfo('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletestuinfo('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='详情' class='nounderline' onclick=\"querystuinfo('" + data + "');\">[详情]</a>&nbsp;" +
                    "<a  title='编辑' class='nounderline' onclick=\"editstuinfo('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoverstuinfo('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_stuinfo").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchstu").click(function () {
    $("#stusearchtype").val("1");
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_stuinfo").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});
$("#btn-advsearchstu").click(function () {
    $("#stusearchtype").val("2");
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_stuinfo").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
    $("#advsearchstu").modal("hide");
})
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
function querystuinfo(data) {
    RedircetUrl(operurl_details + "?stuid=" + data);
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
$("#btn-open-advsearch").click(function () {
    $("#advsearchstu").modal("show");
})
$("#search_grade").change(function () {
    var grade = $("#search_grade").val();
    $("#search_class option").remove();
    $("#search_class").append("<option value='0'>全部</option>");
    if (grade != "0") {
        $.ajax({
            "dataType": 'json',
            "type": "Get",
            "url": getclassbygradeurl,
            "data": { grade: grade },
            "success": function (response) {
                var classobj = response.data;
                $.each(classobj, function (index, item) {
                    $("#search_class").append("<option value='" + item.ClassID + "'>" + item.ClassName + "</option>");
                })

            }
        });
    }
})