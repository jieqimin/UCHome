(function ($) {
    $.fn.SelAddress = function (options) {
        var defaults = {
            defaultprovince: "110000",
            defaultcity: "110100",
            defaultdistrict: "110101",
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
            var jsonObj = { "provincecode": self.settings.defaultprovince };
            $(".addressSelect").append("<select id='ad-province' class = 'form-control' title='选择省'></select><select id='ad-city' class = 'form-control' title='选择市'></select><select class = 'form-control' id='ad-district' title='选择区'></select>");
            $.get("/SchoolInfo/GetProvince", null, function (result) {
                var provinceobj = result;
                $.each(provinceobj, function (index, item) {
                    var provincename = item.Code_Name;
                    var provincecode = item.Code_ID;
                    if (provincecode == self.settings.defaultprovince) {
                        var opt = "<option value='" + provincecode + "' selected>" + provincename + "</option>";
                        self.selectcity(provincecode);
                    }
                    else {
                        var opt = "<option value='" + provincecode + "'>" + provincename + "</option>";
                    }
                    $("#ad-province").append(opt);
                });
            });
            $("#ad-province").click(function () {
                var val = $(this).val(); 
                this.defaultcity = null;
                self.selectcity(val);
            });
            $("#ad-city").click(function () {
                var val = $(this).val();
                this.defaultdistrict = null;
                self.selectdistrict(val);
            });
            $("#ad-district").click(function () {
                var selectvalue = $("#ad-district").val();
                var selecttext = $("#ad-district").find("option:selected").text();
                
                self.callBack(selectvalue, selecttext);
            });

        },
        selectcity: function (provincecode) {
            $("#ad-city option").remove();
            var self = this;
            $.get("/SchoolInfo/GetCity", { "provincecode": provincecode }, function (result) {
                var cityobj = result;
                $.each(cityobj, function (index, item) {
                    var cityname = item.Code_Name;
                    var citycode = item.Code_ID;
                    if (citycode == self.settings.defaultcity||(this.defaultcity==null&&index==0)) {
                        var opt = "<option value='" + citycode + "' selected>" + cityname + "</option>";
                        self.selectdistrict(citycode);
                    }
                    else {
                        var opt = "<option value='" + citycode + "'>" + cityname + "</option>";
                    }
                    $("#ad-city").append(opt);
                });
            });
        },
        selectdistrict: function (citycode) {
            var self = this;
            $("#ad-district option").remove();
            $.get("/SchoolInfo/GetDistrict", { "citycode": citycode }, function (result) {
                var districtobj = result;
                $.each(districtobj, function (index, item) {
                    var districtname = item.Code_Name;
                    var districtcode = item.Code_ID;
                    if (districtcode == self.settings.defaultdistrict || (this.defaultdistrict == null && index == 0)) {
                        var opt = "<option value='" + districtcode + "' selected>" + districtname + "</option>";
                        self.callBack(districtcode, districtname);
                    }
                    else {
                        var opt = "<option value='" + districtcode + "'>" + districtname + "</option>";
                    }
                    $("#ad-district").append(opt);
                });
                //self.setSelectedUser(self.settings.selValue, self.settings.selText);
            });
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
        callBack: function (value, text) {
            if ($.isFunction(this.settings.callBackfn)) {
                this.settings.callBackfn(value, text);
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
