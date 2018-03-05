
$(document).ready(function() {

    //Default Action
    $(".TabMain").hide(); //Hide all content
    $(".TabMenu li:first").addClass("active").show(); //Activate first tab
    $(".TabMain:first").show(); //Show first tab content

    //On Click Event
    $(".TabMenu li").click(function () {
        $(".TabMenu li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".TabMain").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });
});

$(document).ready(function() {

    //Default Action
    $(".CaseMain").hide(); //Hide all content
    $(".CaseMenu li:first").addClass("active").show(); //Activate first tab
    $(".CaseMain:first").show(); //Show first tab content

    //On Click Event
    $(".CaseMenu li").click(function () {
        $(".CaseMenu li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".CaseMain").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });
});

$(document).ready(function() {

    //Default Action
    $(".StatisticsMain").hide(); //Hide all content
    $(".StatisticsMenu li:first").addClass("active").show(); //Activate first tab
    $(".StatisticsMain:first").show(); //Show first tab content

    //On Click Event
    $(".StatisticsMenu li").click(function () {
        $(".StatisticsMenu li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".StatisticsMain").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

});
function scroll() {
    $("#schoolList ul").animate({ "margin-left": "0px" },5000,"swing", function () {
        $("#schoolList ul li:eq(0)").appendTo($("#schoolList ul"));
        $("#schoolList ul").css({ "margin-left": 0 });
    });
}

$(function() {
    setInterval(scroll, 500);
});
