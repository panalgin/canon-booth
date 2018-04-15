var data = unescape("{0}");
$("input#playerInstance").val(data);

var player = JSON.parse(data);

var speed = (Math.round(player.speed * 100) / 100).toFixed(2);
var calorie = (Math.round(player.caloriesBurnt * 100) / 100).toFixed(2);

$("span#speed-label").html(speed + " km/h");
$("span#energy-label").html(calorie + " kcal");

var batteryLevel = map(player.caloriesBurnt, 0, 30, 0, 250);

if (batteryLevel > 250)
    batteryLevel = 250;

$("div.battery-overlay").css("height", batteryLevel + "px");