$("div.container").hide();
$("div#block-input #laterOn").hide();
$("div#block-input").show();

setTimeout(function () {{
    $("div#block-input p").hide();
    $("div#block-input #laterOn").show();
}}, 10000);
