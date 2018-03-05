$(document).ready(function () {
    debugger;
	//banner轮播初始化
	Slider.init({
	    target: $('.slide'),
		time: 6000
	});
	//调整导航位置
	var $slider = $('.slider-nav-container');
	var $sliderCon = $slider.parent();
	var slider_left = ($sliderCon.width()-$slider.width())/2;
	$slider.css("left",slider_left);
}) 