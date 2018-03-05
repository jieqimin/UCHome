$.dataTablesSettings = {
    processing: false,//是否显示加载中提示
    bAutoWidth: false,//是否自动计算表格各列宽度
    bPaginate: false,//是否显示使用分页
    bInfo: false,//是否显示页数信息
    sPaginationType: "full_numbers",//分页样式
    iDisplayLength: 10,//默认每页显示多少条记录
    searching: false,//是否显示搜索框
    bSort: true,//是否允许排序
    serverSide: true,//是否从服务器获取数据
    bStateSave: false,//页面重载后保持当前页
    bLengthChange: false,//是否显示每页大小的下拉框
    sServerMethod: "POST",
    "sScrollX": "100%",
    "sScrollXInner": "100%",
    "bScrollCollapse": true,
    language: {
        lengthMenu: "每页显示 _MENU_记录",
        zeroRecords: "正在加载数据",
        info: "第_PAGE_页/共 _PAGES_页",
        infoEmpty: "没有符合条件的记录",
        search: "查找",
        infoFiltered: "(从 _MAX_条记录中过滤)",
        paginate: { "first": "首页 ", "last": "末页", "next": "下一页", "previous": "上一页" }
    },
    //这里是为ajax添加自定义参数，给它添加的属性，它会传给后台
    //fnServerParams: function (aoData) {
    //    aoData._rand = Math.random();
    //}
}
//一、动态加载数据
//ajax的url
//$.dataTablesSettings.ajax = "/backend/content/load";
//二、如果表格的高度大于这个值，tbody就会出现滚动条，而表头固定
$.dataTablesSettings.sScrollY=$(window).height()-176;//是否开启垂直滚动(否=disabled)
//$.dataTablesSettings.sScrollY="auto";//是否开启垂直滚动(否=disabled)
//三、自定义表格显示列
    //设置具体的列名
    //$.dataTablesSettings.columns = [
    //    { data: "id" },
    //    { data: "title" },
    //    { data: "sort" },
    //    { data: "diffcity" },
    //    { data: "citys" },
    //    { data: "edittime" },
    //    { data: "editer" }
    //];
    ////对列进行操作，这里只是替换掉第一列
    //$.dataTablesSettings.columnDefs = [{
    //    targets: [0],
    //    data: "id",
    //    render: function (data, type, row) {
    //        return "<a title='编辑' class='glyphicon glyphicon-edit nounderline' href='javascript:editTabRow(" + data + ");'></a>&nbsp;" +
    //            "<a title='复制' class='glyphicon glyphicon-duplicate nounderline' href='javascript:copyTabRow(" + data + ");'></a>&nbsp;" +
    //            "<a title='删除' class='glyphicon glyphicon-trash nounderline' href='javascript:deleteTabRow(" + data + ");'></a>";
    //    }
    //}];
//四、事件
    //$.dataTablesSettings.fnDrawCallback = function (data) {
    //    //每一次表格绘制完成时调用
    //};
    //$.dataTablesSettings.fnInitComplete = function () {
    //    //表格初始化时调用一次
    //};
//五、方法
////重绘方法。true会回到表格的初始状态，例如回到第一页，false只是重新加载当前页
//dataTable.fnDraw(false);
////销毁方法。true会删除表格元素，而false不会，它只是销毁dataTable对象
//dataTable.fnDestroy(false);
////换页方法，可以跳转到指定页。可选参数有"first", "previous", "next" , "last"，或者是一个整数，0是第一页
//dataTable.fnPageChange(0);

//六、搜索案例
//  $("#searchBtn").on("click", function () {
//    //这里是为了解决搜索条件变化后，没有点搜索按钮，而是点换页后搜索条件也变化的bug
//    var stitle = $("#stitle").val();
//    var sdiffcity = $("#sdiffcity").val();
//    var scity = $("#scity").val();

//    //这里重新设置参数
//    $.dataTablesSettings.fnServerParams = function (aoData) {
//        aoData.stitle = stitle;
//        aoData.sdiffcity = sdiffcity;
//        aoData.scity = scity;
//        aoData._rand = Math.random();
//    }
//    //搜索就是设置参数，然后销毁datatable重新再建一个
//    dataTable.fnDestroy(false);
//    dataTable = $("#table_users").dataTable($.dataTablesSettings);
//    //搜索后跳转到第一页
//    dataTable.fnPageChange(0);
//});
//七、注意事项
//  在删除的ajax的success方法里需要这么写，如果当前页只有一条数据，删除后跳转到前一页
//  start = $("#datatable").dataTable().fnSettings()._iDisplayStart;
//  total = $("#datatable").dataTable().fnSettings().fnRecordsDisplay();
//  dataTable.fnDraw(false);
//  if ((total - start) == 1) {
//      if (start > 0) {
//          $("#datatable").dataTable().fnPageChange("previous", true);
//      }
//  }
//八、初始化应用
//$("#dataTableId").dataTable($.dataTablesSettings);