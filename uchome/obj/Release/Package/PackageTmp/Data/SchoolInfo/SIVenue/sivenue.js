var venuetypes;
var grounds
$.get(venuetypegeturl, function (data) {
    venuetypes = data;
})
$.get(venuegroundgeturl, function (data) {
    grounds = data;
})
$.dataTablesSettings.ajax = {
    dataSrc: "data"//用于指定需要绑定的ajax请求返回的结果数据源集合
};
$.dataTablesSettings.fnServerData = function (sSource, aoData, fnCallback) {
    var groundid = $("#search_ground").val();
    $.ajax({
        "dataType": 'json',
        "type": "Get",
        "url": initlisturl,
        "data": { groundid: groundid},
        "success": function (response) {
            $("#records").text(response.recordcount);
            fnCallback(response);
        }
    })
}
$.dataTablesSettings.aoColumns = [
    { data: "VenueID_GroundID" },
    { data: "VenueType_DictCode" },
    { data: "VenueName" },
    { data: "VenueMaxPersons" },
    { data: "IsState" },
    { data: "VenueID" }
];
//对列进行操作，这里只是替换掉第一列
$.dataTablesSettings.columnDefs = [
    {
        targets: [0],
        data: "VenueID_GroundID",
        render: function (data, type, row) {
            var ground = $.grep(grounds, function (item) {
                return item.GroundID == data;
            })
            if (ground.length == 1)
                return ground[0].GroundName;
            else
                return "未找到";
        }
    }, 
    {
        targets: [1],
        data: "VenueType_DictCode",
        render: function (data, type, row) {
            var venuetype = $.grep(venuetypes, function (item) {
                return item.DictCode == data;
            })
            if (venuetype.length == 1)
                return venuetype[0].DictName;
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
        data: "VenueID",
        render: function (data, type, row) {
            if (row.IsState == 1) {
                return "<a  title='编辑' class='nounderline' onclick=\"editsivenue('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='禁用' class='nounderline' onclick=\"deletesivenue('" + data + "');\">[删除]</a>";
            }
            else {
                return "<a  title='编辑' class='nounderline' onclick=\"editsivenue('" + data + "');\">[编辑]</a>&nbsp;" +
                    "<a  title='恢复' class='nounderline' onclick=\"rescoversivenue('" + data + "');\">[恢复]</a>";
            }
        }
    }];
$.dataTablesSettings.bSort = false;
var userdataTable = $("#table_sivenue").dataTable($.dataTablesSettings);;

//这里是搜索的案例，不过自定义分页也可以这么做
$("#btn-searchsivenue").click(function () {
    userdataTable.fnDestroy(false);
    userdataTable = $("#table_sivenue").dataTable($.dataTablesSettings);
    //搜索后跳转到第一页
    userdataTable.fnPageChange(0);
});

function deletesivenue(data) {
    $.ajax({
        type: "POST",
        url: operurl_delete,
        data: { "venueid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("删除成功！")
                $("#table_sivenue").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("删除失败！")
            }
        }
    })

}

function editsivenue(data) {
    RedircetUrl(operurl_edit + "?venueid=" + data);
}
function rescoversivenue(data) {
    $.ajax({
        type: "POST",
        url: operurl_recovery,
        data: { "venueid": data },
        success: function (result) {
            if (result.statuscode == 200) {
                userdataTable.fnDestroy(false);
                tipsuccessmsg("恢复成功！")
                $("#table_sivenue").dataTable($.dataTablesSettings).fnDraw(false);
            }
            else {
                tipsuccessmsg("恢复失败！")
            }
        }
    })
}