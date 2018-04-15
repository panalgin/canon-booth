var path = unescape("{0}");

var data = unescape($("input#playerInstance").val());
var player = JSON.parse(data);

$("div#loading").hide();
$("img#result-gif").prop("src", path);
$("p#result-name").html(player.firstName);
$("div#result").show();