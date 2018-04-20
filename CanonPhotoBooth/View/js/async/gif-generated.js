var path = unescape("{0}");

var data = unescape($("input#playerInstance").val());
var player = JSON.parse(data);

$("div#loading").hide();
$("img#result-gif").prop("src", path);
$("div#result").show();