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

var _hmt = _hmt || [];
(function() {
  var hm = document.createElement("script");
  hm.src = "//hm.baidu.com/hm.js?a66404e3cc462a648464a5536d52bfb7";
  var s = document.getElementsByTagName("script")[0]; 
  s.parentNode.insertBefore(hm, s);
})();
(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-77459554-1', 'auto');
  ga('send', 'pageview');
