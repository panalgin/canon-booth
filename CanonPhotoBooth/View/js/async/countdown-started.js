
$("p#counter-label").css("font-size", "100px").css("-webkit-text-stroke", "#000 4px").html("READY!");

var counter = 3;

var coutndown = setInterval(function () {
    if (counter != 0) {
        $("p#counter-label").html(counter);

        counter--;
    }
    else {
        $("p#counter-label").html("GO!");
        clearInterval(countdown);
    }
}, 1000);