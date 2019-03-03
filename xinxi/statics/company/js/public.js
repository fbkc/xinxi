$(function () {
    $(".shutdown").click(function () {
        $("#window").fadeOut(500);
    });

    $(".music").click(function () {
        if ($(this).hasClass("pause")) {
            $("audio").trigger('play');
        } else {
            $("audio").trigger('pause');
        }
        $(this).toggleClass("pause");
    });
});

function cNav(nav) {
    var str = '';
    console.log(nav);
    for (var i = 0; i < nav.length; i++) {
        str += '<li><a rel="nofollow" href="' + nav[i].navUrl + '">' + nav[i].navName + '</a></li>';
    }
    console.log(str);
    return str;
}
function goWebSite(mobileWebSite) {
    mobileWebSite = decode(mobileWebSite);
    mobileWebSite = decode(mobileWebSite);
    document.body.appendChild(document.createElement('iframe')).src = 'javascript:"<script>top.location.replace(\'' + mobileWebSite + '\')<\/script>"';
}

function decode(data) {
    var a = CryptoJS.enc.Base64.parse(data);
    return a.toString(CryptoJS.enc.Utf8);
}
function showNavMenu() {
    $("#nav").toggle(500);
}
today = new Date();
var day = '',
    date = '';
if (today.getDay() == 0) day = " 鏄熸湡鏃�";
if (today.getDay() == 1) day = " 鏄熸湡涓€";
if (today.getDay() == 2) day = " 鏄熸湡浜�";
if (today.getDay() == 3) day = " 鏄熸湡涓�";
if (today.getDay() == 4) day = " 鏄熸湡鍥�";
if (today.getDay() == 5) day = " 鏄熸湡浜�";
if (today.getDay() == 6) day = " 鏄熸湡鍏�";
date = "浠婂ぉ鏄細" + (today.getFullYear()) + "骞�" + (today.getMonth() + 1) + "鏈�" + today.getDate() + "鏃�" + day + "";
$("#time").text(date);

function closeWindow() {
    $("#window").fadeOut();
}