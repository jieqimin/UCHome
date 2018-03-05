function delconfirm(tiptxt, callback, opts) {
    var obj = "<div id='delconfirm' title='操作提示' role='dialog'>";
    obj += "<div class='modal-body'><p class='text-center'>" + tiptxt + "</p></div>";
    obj += "<div class='modal-footer'><button id='btn-confirm-sure' class='btn btn-danger' data-dismiss='modal' type='button'>确定</button><button id='btn-confirm-cancel' class='btn btn-default' data-dismiss='modal' type='button'>取消</button></div>";
    obj += "</div></div>";
    $("body").append($(obj));
    $("#delconfirm").dialog({ modal: true });
    $('#btn-confirm-sure').click(function () {
        callback.call(this, opts);
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#delconfirm").remove();
    });
    $("#btn-confirm-cancel").click(function () {
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#delconfirm").remove();
    });
}
function completeselect(tiptxt, callback1, callback2, opts) {
    var obj = "<div id='completeselect' title='操作提示' role='dialog'>";
    obj += "<div class='modal-body'><p class='text-center'>" + tiptxt + "</p></div>";
    obj += "<div class='modal-footer'><button id='btn-confirm-goon' class='btn btn-danger' data-dismiss='modal' type='button'>继续添加</button><button id='btn-confirm-return' class='btn btn-default'  type='button'>返回列表</button></div>";
    obj += "</div></div>";
    $("body").append($(obj));
    $("#completeselect").dialog({ modal: true });
    $('#btn-confirm-goon').click(function () {
        callback1.call(this, opts);
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#completeselect").remove();
    });
    $("#btn-confirm-return").click(function () {
        callback2.call(this, opts);
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#completeselect").remove();
    });
}
function opermsgtip(tiptxt, callback, opts) {

   toastr.options = {
        "positionClass": "toast-top-center"
    };
    toastr["success"](tiptxt);
    callback.call(this, opts);
}
function setsharevalue(tiptxt, callback, opts) {
    var obj = "<div id='sharesetting' class='modal fade show bs-example-modal-sm' role='dialog'>";
    obj += "<div class='modal-dialog modal-lg'><div class='modal-content'>";
    obj += "<div class='modal-header'><h4 class='modal-title'>权限设置</h4></div>";
    obj += "<div class='modal-body text-center'>";
    obj += "<input type='radio' name='roleradio' value='0'> 个人私有 ";
    obj += "<input type='radio' name='roleradio' value='1'> 对好友公开 ";
    obj += "<input type='radio' name='roleradio' value='2'> 对同学公开 ";
    obj += "<input type='radio' name='roleradio' value='3'> 对老师公开 ";
    obj += "<input type='radio' name='roleradio' value='4'> 对学校公开 ";
    obj += "<input type='radio' name='roleradio' value='5'> 对本区公开 ";
    obj += "<input type='radio' name='roleradio' value='6'> 对全市公开 ";
    obj += "<input type='radio' name='roleradio' value='9'> 完全公开 ";
    obj += "</div>";
    obj += "<div class='modal-footer'><button id='btn-confirm-sure' class='btn btn-danger' data-dismiss='modal' type='button'>确定</button><button id='btn-confirm-cancel' class='btn btn-default' data-dismiss='modal' type='button'>取消</button></div>";
    obj += "</div></div></div>";
    $("body").append($(obj));
    $('#btn-confirm-sure').click(function () {
        callback.call(this, opts, $("input:radio[name=roleradio]:checked").val());
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#sharesetting").remove();
    });
    $("#btn-confirm-cancel").click(function () {
        $("body").removeClass("modal-open");
        $(".modal-backdrop").remove();
        $("#sharesetting").remove();
    });
}
function tipsuccessmsg(msg) {
    toastr.options = {
        "positionClass": "toast-top-center"
    };
    toastr.success(msg);
}
function tipfailmsg(msg) {
    toastr.options = {
        "positionClass": "toast-top-center"
    };
    toastr.error(msg);
}
function tipwarningmsg(msg) {
    toastr.options = {
        "positionClass": "toast-top-center"
    };
    toastr.warning(msg);
}
function opersuccesstip(obj, msg, type) {
    tipsuccessmsg(msg);
}
function operfailtip(obj, msg, type) {
    tipfailmsg(msg);
}
function hidesuccessmsg(seconds) {

}
function hidefailmsg(seconds) {

}
function hidesuccesstip(seconds) {

}
function hidefailtip(seconds) {

}
function alertmsg(type, msg) {
    var obj;
    if (type == "success")
        obj = "<div id='alertinfo' class='alertbg alert alert-success'><a href='#' class='close' data-dismiss='alert'>&times;</a>" + msg + "</div>";
    else if (type == "warning")
        obj = "<div id='alertinfo' class='alertbg alert alert-warning'><a href='#' class='close' data-dismiss='alert'>&times;</a>" + msg + "</div>";
    else if (type == "fail")
        obj = "<div id='alertinfo' class='alertbg alert alert-danger'><a href='#' class='close' data-dismiss='alert'>&times;</a>" + msg + "</div>";
    else
        obj = "<div id='alertinfo' class='alertbg alert alert-info'><a href='#' class='close' data-dismiss='alert'>&times;</a>" + msg + "</div>";
    $("body").append(obj);
    var fadediv = "<div class='modal-backdrop fade in'></div>";
    $("body").append(fadediv);
    $("#alertinfo").bind('closed.bs.alert', function () {
        $(".modal-backdrop").remove();
    });
    setTimeout(function () { $("#alertinfo").alert('close'); $(".modal-backdrop").remove(); }, 1000);

}
function loaddata(obj) {
    var loadhtml = "<div class='loading'>正在加载中，请稍候…</div>";
    $(obj).html(loadhtml);
}
function loaddata2(obj) {
    var loadhtml = "<div class='loading loading_green'>正在加载中，请稍候…</div>";
    $(obj).html(loadhtml);
}
function hideload(obj) {
    if (obj != null)
        $(obj).find($(".loading")).remove();
    else
        $(".loading").remove();
}

function nodatatip(msg) {
    return "<p class='nodata'>" + msg + "</p>";
}

function buildingtip(msg) {

}

function errortip(msg) {

}