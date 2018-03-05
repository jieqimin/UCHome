// 文件上传
function Upfiles(options) {
    this.upfilelist = new Array();//this写法表示私有
    this.defaultfiles = null;
    this.returnvalue = null;
    this.id = null;
    this.baseurl = "/plugs/webuploader/";
    this.serverurl = "/UCHome/PublicShare/AddFile";
    this.callbackfn = null;
    this.tip = "【提示：单个文件请不要超过5M】";
    this.init(options);
}
Upfiles.prototype = {
    //upfilelist:new Array(),//这个写法不属于私有，同一个页面如果有多个组件容易数据混淆
    init: function (options) {
        var self = this;
        this.id = options.id;
        this.baseurl = options.baseurl;
        this.serverurl = options.serverurl;
        this.defaultfiles = options.files;
        this.callbackfn = options.callbackfn;
        this.tip = typeof(options.tip) == "undefined" ? this.tip : options.tip;
        this.CreateDom();
        $("#" + this.id + "sureBtn").on("click", function () {
            self.callback(self.upfilelist, self.callbackfn);
        });
    },
    callback: function (returnvalue, callbackfn) {
        if ($.isFunction(callbackfn)) {
            this.callbackfn(JSON.stringify(returnvalue));
        }
    },
    CreateDom: function () {
        var self = this;
        var upfileid = this.id;
        var obj = "<div class=\"btns\">";
        obj += "<div class=\"pull-left\" id=\"" + upfileid + "picker\"><div style='border:0;width:100px;height:80px;'></div></div>";
        //obj += "<span class=\"pull-left help-block\">" + this.tip + "</span>";
        obj += "</div>";
        obj += "<div class=\"modal fade\" id=\"" + upfileid + "UpFileModal\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\">";
        obj += "    <div class=\"modal-dialog\" role=\"document\">";
        obj += "   <div class=\"modal-content\">";
        obj += "  <div class=\"modal-header\">";
        obj += " <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
        obj += " <h4 class=\"modal-title\" id=\"myModalLabel\">上传文件列表</h4>";
        obj += "  </div>";
        obj += "  <div class=\"modal-body\">";
        obj += " <div id=\"" + upfileid + "thelist\" class=\"uploader-list form-horizontal\"></div>";
        obj += "  </div>";
        obj += "  <div class=\"modal-footer\">";
        obj += " <button id=\"" + upfileid + "ctlBtn\" type=\"button\" class=\"btn btn-info\">开始上传</button>";
        obj += " <button id=\"" + upfileid + "sureBtn\" type=\"button\" class=\"btn btn-success upfile-btnsure\" data-dismiss=\"modal\">确定</button>";
        obj += "  </div>";
        obj += "   </div>";
        obj += "    </div>";
        obj += "</div>";
        $("#" + upfileid).append($(obj));
        $(".btns").css("position", "absolute").css("top", 0).css("left",0);
        var $upfile = $("#" + upfileid);
        var $list = $('#' + upfileid + 'thelist');
        var $btn = $('#' + upfileid + 'ctlBtn');
        var state = 'pending';
        var uploader = new WebUploader.Uploader({
            swf: self.baseurl + '/js/Uploader.swf',
            server: self.serverurl,
            pick: "#" + upfileid + "picker",
            resize: false,
            fileNumLimit: 300,
            fileSizeLimit: 500 * 1024 * 1024,    // 200 M
            fileSingleSizeLimit: 500 * 1024 * 1024,   // 50 M
            accept: {
                title: 'Images',
                extensions: 'gif,jpg,jpeg,bmp,png',
                mimeTypes: 'image/*'
            }
        });
        //console.log(this.defaultfiles);
        if (this.defaultfiles != null) {
            $.each(this.defaultfiles, function (index, item) {
                var filehtml = '<div id="' + item.fileid + '" filepath="' + item.filepath + '" class="item form-group">' +
                    '<span class="info col-sm-9">' + item.filename + '</span>' +
                    '<p class="state col-sm-2">已上传</p>' +
                    '</div>';
                var f2 = { "fileid": item.fileid, "filename": item.filename, "filepath": item.filepath };
                alert(1);
                self.upfilelist.push(f2);
                $list.append(filehtml);
            });
            //$upfile.append("<button id='" + upfileid + "filequery' type='button' class='btn btn-warning' data-toggle='modal' data-target='#" + upfileid + "UpFileModal'>查看文件</button>");
        }
        uploader.on('fileQueued', function (file) {
            //$list.append('<div id="' + file.id + '" class="item form-group">' +
            //    '<span class="info col-sm-9">' + file.name + '</span>' +
            //    '<p class="state col-sm-2">等待上传...</p>' +
            //'</div>');
            //$("#" + upfileid + "UpFileModal").modal("show");
            uploader.upload();
        });

        // 文件上传过程中创建进度条实时显示。
        uploader.on('uploadProgress', function (file, percentage) {
            var $li = $('#' + file.id),
                $percent = $li.find('.progress .progress-bar');

            // 避免重复创建
            if (!$percent.length) {
                $percent = $('<div class="progress progress-striped active col-sm-12">' +
                  '<div class="progress-bar" role="progressbar" style="width: 0%">' +
                  '</div>' +
                '</div>').appendTo($li).find('.progress-bar');
            }

            $li.find('p.state').text('上传中');

            $percent.css('width', percentage * 100 + '%');
        });
        uploader.on("uploadAccept", function (object, ret) {
            if (ret != null && ret.Data != "undefined") {
                var statuscode = ret.Data.statuscode;
                if (statuscode == 200) {
                    var f = { "fileid": ret.Data.fileid, "filename": ret.Data.filename, "filepath": ret.Data.filepath };
                    self.upfilelist.push(f);
                }
            }           
            //if ($("#" + upfileid + "filequery").length == 0)
            //    $upfile.append("<button id='" + upfileid + "filequery' type='button' class='btn btn-warning' data-toggle='modal' data-target='#" + upfileid + "UpFileModal'>查看文件</button>");
        });
        uploader.on('uploadSuccess', function (file) {
            $('#' + file.id).find('p.state').text('已上传');
        });

        uploader.on('uploadError', function (file) {
            $('#' + file.id).find('p.state').text('上传出错');
        });

        uploader.on('uploadComplete', function (file) {
            $('#' + file.id).find('.progress').fadeOut();
            self.callback(self.upfilelist, self.callbackfn);
        });

        uploader.on('all', function (type) {
            if (type === 'startUpload') {
                state = 'uploading';
            } else if (type === 'stopUpload') {
                state = 'paused';
            } else if (type === 'uploadFinished') {
                state = 'done';
            }
            if (state === 'uploading') {
                $btn.text('暂停上传');
            } else {
                $btn.text('开始上传');
            }
        });

        $btn.on('click', function () {
            if (state === 'uploading') {
                uploader.stop();
            } else {
                uploader.upload();
            }
        });

    }
}

jQuery(function () {
    var $ = jQuery;
    // 当有文件添加进来的时候 
});
