//选人控件属性：
//selRole:【tea:老师,stu:学生,fam:家长】
//selType:【muti:多选,single:单选】
//selArea:【friend:我的好友,student:我的学生,stuparent:学生家长,colleague:我的同事,myclassmate:我的同学,teacher:我的老师,classmateparent:同学家长,department:学校部门】
//selValue：已选中的人员
(function ($) {
    $.fn.SelUser = function (options) {
        var defaults = {
            selRole: "tea",
            selType: "muti",
            selArea: "friend",
            selValue: null,
            selText: null,
            callBackfn: null,
            event: 'show'
        },
            settings = $.extend({}, this.defaults, options);
        return this.each(function () {
            return new _SelUser(this, settings);
        });
    }
    var _SelUser = function (el, settings) {
        this.$seluser = $(el);
        this.settings = settings;
        this.init();
    }
    _SelUser.prototype = {
        init: function () {
            var _self = this;
            _self.show();
        },
        show: function () {
            var self = this;
            var jsonObj = { "selRole": self.settings.selRole, "selType": self.settings.selType, "selArea": self.settings.selArea };
            self.cleardialog();
            event.preventDefault();
            $("body").append("<div id='UserSelectModal' title='选择人员'></div>");
            self.showDialog("#UserSelectModal");
            $("#UserSelectModal").load("/UserControl/SelectUser", jsonObj, function () {
                self.setSelectedUser(self.settings.selValue, self.settings.selText);
            });

        },
        showDialog: function (selobj) {
            var self = this;
            $(selobj).dialog({
                autoOpen: true,
                modal: true,
                width: 644,
                title: "人员选择",
                buttons: {
                    "清空": function () {
                        $("#ulSelectedUser li").remove();
                        $("#ulUserList a").css("color", "#404040").find("span").removeClass("glyphicon-ok").addClass("glyphicon-arrow-right").css("color", "#ddd");
                    },
                    "确定": function () {
                        var selvalues = getSelectUserInfo();
                        self.callBack(selvalues[0], selvalues[1]);
                        $(selobj).dialog("close");
                    },
                    "关闭": function () {
                        self.cleardialog();
                        $(selobj).dialog("close");
                    }

                }
            });
            $(".ui-dialog-title").after("<span style='color:#F77E27;font-size:14px;line-height:42px;font-family:微软雅黑;font-weight:normal;'>（操作提示：双击左边人员分组可以全选该组人员）</span>")
        },
        setSelectedUser: function (objA, objB) {
            var ids = objA;
            var names = objB;
            //$("#ulSelectedUser").remove();
            $("#user_selitem").append("<ul class=\"listbox1\" id=\"ulSelectedUser\"></ul>");
            if (ids.length > 0) {
                var userids = ids.split(',');
                var usernames = names.split(',');
                for (var i = 0; i < userids.length; i++) {
                    var strLi = "<li onclick='clearData(this)' class=\"list-group-item-info\"><span class=\"spname\" id=\"" + userids[i] + "\">" + usernames[i] + "</span>" +
                        "<span class='spbtn'><button type='button' class='close' data-dismiss='modal' aria-label='Close'>" +
                        "<span aria-hidden='true' class='spclose'>×</span>" +
                        "</button></span></li>";
                    $("#ulSelectedUser").append(strLi);
                    $("#ulUserList #" + userids[i]).css("color", "green").find("span").removeClass("glyphicon-arrow-right").addClass("glyphicon-ok").css("color", "green");
                }
            }
        },
        callBack: function (ids, names) {
            if ($.isFunction(this.settings.callBackfn)) {
                this.settings.callBackfn(ids, names);
            }
        },
        cleardialog: function () {
            $(".ui-dialog").each(function () {
                $(this).children().remove();
                $(this).remove();
            });
        },
        otherMethod: function () { }
    };

    function getSelectUserInfo() {
        var ids = "", names = "";
        //取li下面所有 classname为spname的对象
        $("#ulSelectedUser li .spname").each(function () {
            ids += "," + $(this).attr("id");
            names += "," + $(this).text();
        });
        if (ids.length > 0) {
            ids = ids.substring(1, ids.length);
            names = names.substring(1, names.length);
        }
        return [ids, names];
    }

}(jQuery))
