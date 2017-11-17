//weather.js

var weatherCallback = function (data) {
    var wind = data.query.results.channel.wind;
    var item = data.query.results.channel.item;
    var text = "Temperature: " + item.condition.temp + " °C";
    $("#temperatureDiv p").html(text);

    if (item.condition.temp <= 20) {
        updateTemperatureDiv("The cold doesn't bother, we are the IDF!");
    }
    else if (item.condition.temp > 16 && item.condition.temp < 30) {
        updateTemperatureDiv("The weather is amazing! now get back to work!");
    } else {
        updateTemperatureDiv("Sweating will only make your stronger!");
    }
};

function updateTemperatureDiv(text) {
    $("#temperatureDiv").append("<p>" + text + "</p>");
}