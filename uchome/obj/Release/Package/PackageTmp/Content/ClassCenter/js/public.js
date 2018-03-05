// JavaScript Document

$(function(){
	//总导航
    $(".navv li").click(function () {
		$(this).siblings().removeClass("nav-check");
		$(this).addClass("nav-check");
		})
	//图中学校和年级班级的居中
     var a=$(".school").text().length;
	 var b=a*55;
	 $(".school-lesson").width(b);	
	 //内容导航切换
	 $(".connav li").click(function(){
		$(this).siblings().removeClass("connav-check");
		$(this).addClass("connav-check");
		})
	})