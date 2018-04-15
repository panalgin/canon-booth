var data = unescape("{0}");
$("input#playerInstance").val(data);

var player = JSON.parse(data);

$("span#speed-label").html(player.speed + " km/h");
$("span#energy-label").html(player.caloriesBurnt + " kcal");

var batteryLevel = map(player.caloriesBurnt, 0, 20, 0, 200);
var videoSpeed = map(player.speed, 0, 50, 0.01, 5);

var videoPlayer = document.getElementById("video-player");

videoPlayer.playbackRate = videoSpeed;
$("div.battery-overlay").css("height", batteryLevel + "px");