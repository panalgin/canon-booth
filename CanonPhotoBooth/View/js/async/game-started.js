$("div#pre-play").hide();

var data = unescape($("input#playerInstance").val());
var player = JSON.parse(data);

$("span#name-label").html("Player: " + player.firstName);
$("span#speed-label").html(player.speed + " km/h");
$("span#time-left-label").html("~");
$("span#energy-label").html(player.caloriesBurnt + " kcal");

var batteryLevel = map(player.caloriesBurnt, 0, 20, 0, 200);
//var videoSpeed = map(player.speed, 0, 50, 0.05, 5);

//var videoPlayer = document.getElementById("video-player");

//videoPlayer.playbackRate = videoSpeed;
$("div.battery-overlay").css("height", batteryLevel + "px");

$("div#play").show();