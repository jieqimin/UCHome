// JavaScript Document
	$(function(){
			//轮播调用
			DY_scroll('.banner-all','.left','.right','.small-banner',3,false);// true为自动播放，不加此参数或false就默认不自动
			
			//banner切换
			$(".small-banner .small-item").click(function(){
				$(".big-banner a img:eq("+$(this).attr("data-id")+")").removeAttr("hidden").siblings().attr("hidden","true");
			});
			
			
		});
	//轮播方法
	function DY_scroll(wraper,prev,next,img,speed,or)
		   { 
			var wraper = $(wraper);
			var prev = $(prev);
			var next = $(next);
			var img = $(img).find('ul');
			var w = img.find('li').outerWidth(true);
			var s = speed;
			next.click(function()
				 {
				  img.animate({'margin-left':-w},function()
							{
							 img.find('li').eq(0).appendTo(img);
							 img.css({'margin-left':0});
							 });
				  });
			prev.click(function()
				 {
				  img.find('li:last').prependTo(img);
				  img.css({'margin-left':-w});
				  img.animate({'margin-left':0});
				  });
			if (or == true)
			{
			 ad = setInterval(function() { next.click();},s*2000);
			 wraper.hover(function(){clearInterval(ad);},function(){ad = setInterval(function() { next.click();},s*2000);});
		  
			}
		   }
	
