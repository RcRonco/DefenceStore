//weather.js

var weatherCallback = function (data) {
    var wind = data.query.results.channel.wind;
    var item = data.query.results.channel.item;
    var text = "Temperature: " + item.condition.temp + " °C";
    $("#temperatureDiv p").html(text);

    if (item.condition.temp <= 20) {
        updateTemperatureDiv("Its pretty cold toady, maybe you would like to order some warm clothes?");
    }
    else if (item.condition.temp > 16 && item.condition.temp < 30) {
        updateTemperatureDiv("The weather is great! but you are still a soldier so that sucks.");
    } else {
        updateTemperatureDiv("Sweating will only mkae your stronger!");
    }
};

function updateTemperatureDiv(text) {
    $("#temperatureDiv").append("<p>" + text + "</p>");
}