$(document).ready(function () {
    //Add the hover affect to the nptHeaderMenu
    $("#LowerHeader div").hover(function () {
        $(this).addClass("hover");
    });

    $("#LowerHeader div").mouseleave(function () {
        $(this).removeClass("hover");
    });

    //Add the hover affect to the nptHeaderMenu
    $("#HeaderMenu li").hover(function () {
        $(this).addClass("hover");
    });

    $("#HeaderMenu li").mouseleave(function () {
        $(this).removeClass("hover");
    });
});