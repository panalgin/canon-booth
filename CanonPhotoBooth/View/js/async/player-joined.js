var data = "{0}";
$("input#playerInstance").val(data);

var task = JSON.parse(unescape(data));

$("div#holding").hide();

$("p#player-name").html("Welcome " + task.firstName);
$("p#counter-label").css("font-size", "60px").css("-webkit-text-stroke", "#000 2px").html("Waiting next player");
$("div#pre-play").show();
