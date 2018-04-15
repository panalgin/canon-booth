var data = "{0}";
var isWinner = data === "True";

if (isWinner)
    $("span#result-message").css("font-size", "120px").html("<br />YOU<br />WIN!");
else
    $("span#result-message").css("font-size", "110px");

$("div#play").hide();
$("div#loading").show();