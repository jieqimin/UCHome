function openwindow(url, w, h, title) {
    $.layer({
        type: 2,
        closeBtn: false,
        shadeClose: true,
        time: 1,
        end: function () {
            $.layer({
                type: 2,
                title: title,
                shadeClose: true,
                maxmin: true,
                fix: false,
                area: [w, h],
                iframe: {
                    src: url
                }
            });
        }
    });
}

function tabtoggle(tabname, contentname,tabid) {
    $("." + tabname).removeClass("selected");
    $("#" + tabid).addClass("selected");
    $("." +contentname+"_"+tabname).hide();
    $("#" + contentname + "_" + tabid).show();

}