var rem=20;
function resizeRem(){
	rem=20/320*document.documentElement.clientWidth;
	document.documentElement.style.fontSize=rem+"px";
}
document.addEventListener("DOMContentLoaded",function(){
	resizeRem();
},false)
window.onresize=function(){
	resizeRem();
}
