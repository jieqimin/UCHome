

// this code creates the Closure structure
(function (jQuery) {
    $.fn.templatepager = function (options) {
        var options = jQuery.extend({
            pagerData: {
                pageIndex: 0,
                pageSize: 10
            },
            url: "",
            tmpl: "#tmpl",
            databind: function (content) {
            },
            initData: function (ret) { return ret; },
            initResult: function (result) { },
            content: ".content",
            appendTo: true,
            pager: ".pager",
            data: "data",
            total: "total",
            first: "首页",
            prev: "",
            next: "",
            last: "末页",
            loadingHtml: "<div class='progress progress-striped active' style='position: relative;top: 44%;width: 70%;margin: auto;'><div class='progress-bar progress-bar-info'  style='width: 100%'></div></div>"
        }, options);



        // this code snippet will loop through the selected elements and return the jQuery object 
        // when complete
        return this.each(function () {
            // inside each iteration, you can reference the current element by using the standard
            // jQuery(this) notation
            // the rest of the plug-in code goes here
            var self = $(this);

            //var showLoading = function () {
            //    var shade = $("#loading");
            //    var table = self.find("table>tbody");

            //    if (!shade.length) {
            //        var shade = $("<div>", { id: "loading", style: "position: absolute; background-color: rgba(0, 0, 0, 0.1);display:none;" });
            //        shade.append(options.loadingHtml);
            //        shade.css({ "width": table.width(), "height": table.height() });
            //        self.find("table").after(shade);
            //    }

            //    var height = table.height();
            //    if (height == 0)
            //        shade.css({ "height": "390px", "margin-top": "0" });
            //    else
            //        shade.css({ "height": height, "margin-top": -height });

            //    var process = $(shade.children()[0]);
            //    var top = (shade.height() - process.height()) / 2;
            //    process.css("top", parseInt(top) < 0 ? 0 : top);
            //    shade.show();
            //}
            //var hideLoading = function () {
            //    var shade = $("#loading");
            //    if (shade) { shade.hide(); }
            //}
  
            var getRecords = function (index) {
                if (index != undefined) {
                    options.pagerData.pageIndex = index;
                }
                var params = $.extend(options.params, options.pagerData);
                if (options.initParam != undefined) {
                    params = options.initParam(param);
                }
                $.ajax({
                    traditional: true,
                    cache: false,
                    url: options.url,
                    data: params,
                    contentType: "application/json",
                    type: "GET",
                    beforeSend: function () {
                        //showLoading();
                        if (options.begin != undefined) {
                            options.begin();
                        }
                    },
                    success: function (ret) {
                        //hideLoading();
                        var pagerData = options.pagerData;
                        if (options.initData != undefined) {
                            ret = options.initData(ret, pagerData);
                        }

                        //显示分页信息
                        var start = (pagerData.pageIndex * pagerData.pageSize) + 1;
                        //var end = (pageData.pageSize * pageData.pageIndex) > ret.total ? ret.total : pageData.pageSize * pageData.pageIndex;
                        var totalPage = (ret.total / pagerData.pageSize).toString();
                        if (totalPage.indexOf(".") != -1) {
                            var index = totalPage.substr(0).indexOf(".");
                            totalPage = parseInt(totalPage.substr(0, index)) + 1;
                        }
                        var currentPage = ret.total == "0" ? 0 : pagerData.pageIndex;
                        $("#pageinfo").text("共" + totalPage + "页 " + ret.total + "条记录，当前显示为第" + (currentPage + 1) + "页");
                        //共10页 100条记录，当前第显示为第1页
               
                        var content = self.find(options.content);
                        content.empty();
                        if (options.initResult != undefined) {
                            options.initResult(ret);
                        }
                        var renderBody = $(options.tmpl).tmpl(ret[options.data]);
                        if (options.appendTo) {
                            renderBody.appendTo(content);
                        }
                        else {
                            renderBody.prependTo(content);
                        }
                        if (options.databind != undefined) {
                            options.databind(renderBody);
                        }


                        var pageCount = parseInt(ret[options.total] / options.pagerData.pageSize) + (((ret[options.total] % options.pagerData.pageSize) > 0) ? 1 : 0);

                        if (pageCount > 1) {
                            self.find(options.pager).pager({
                                pagenumber: options.pagerData.pageIndex,
                                pagecount: pageCount,
                                buttonClickCallback: getRecords,
                                buttonLabels: {
                                    first: options.first,
                                    prev: options.prev,
                                    next: options.next,
                                    last: options.last
                                }
                            }).show();
                        }
                        else {
                            self.find(options.pager).hide();
                        }

                    },
                    error: function (ret) {
                        if (options.error != undefined) {
                            options.error(ret);
                        }
                    },
                    complete: function () {
                        if (options.complete != undefined) {
                            options.complete(options);
                        }
                        //hideLoading();
                    }
                }).always(function () { App.init(); });
            };
            getRecords();
        });
    }; // don't forget the semi-colon to close the method
    // this code ends the Closure structure
})(jQuery);
