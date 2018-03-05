// JavaScript Document
$(function(){
		//照片墙效果	
    $(".mien>li").hover(function () {
        alert(1);
			$(this).addClass("on");
			$(this).find("img").animate({"width":"200px","height":"150px"});
			$(this).find("a").animate({"width":"210px","height":"160px"});
			
		},function(){
			$(this).animate({height:"89px"}).removeClass("on");
			$(this).find("img").stop(true,true).animate({"width":"118px","height":"89px"});
			$(this).find("a").stop(true,true).animate({"width":"118px","height":"89px"});
	});
});
