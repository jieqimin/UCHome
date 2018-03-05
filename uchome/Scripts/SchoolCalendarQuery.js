// JavaScript Document
$(function() {
	var CurrentDate=new Date();
    var Parse_date=CurrentDate.getTime();
    var Index=0;
    var current_html="";
	$(".List li").each(function(index, element) {
        var span_time = $(this).find(".Title span");
        var time1=span_time.attr("datatime1");
        var time2=span_time.attr("datatime2");
        var Parse_time1=Date.parse(time1);
        var Parse_time2=Date.parse(time2);
        if (Parse_date>=Parse_time1&&Parse_date<=Parse_time2) {
            Index=index;
            $(this).addClass("Current");
        }
    });
    if (Index>0) {
        $(".Current").each(function(){
            current_html += "<li class=\"Current\">" +$(this).html()+"</li>";
        });
        $(this).find(".Current").css("display","none");
        $(".List").prepend(current_html);
    }
})