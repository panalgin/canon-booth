var data = unescape("{0}");
$("input#playerInstance").val(data);

var player = JSON.parse(data);

var speed = (Math.round(player.speed * 100) / 100).toFixed(2);
var calorie = (Math.round(player.caloriesBurnt * 100) / 100).toFixed(2);

if (speed > 30)
    speed = 30;

$("span#speed-label").html(speed + " km/h");
$("span#energy-label").html(calorie + " kcal");

$("span.speed-label").html("Speed: " + speed + "km/h");
$("span.energy-label").html("Energy Generated: " + calorie + " kcal");


if (player.caloriesBurnt > 10)
    player.caloriesBurnt = 10;

var batteryLevel = map(player.caloriesBurnt, 0, 10, 0, 210);

if (batteryLevel > 210)
    batteryLevel = 210;

$("div.battery-overlay").css("height", batteryLevel + "px");