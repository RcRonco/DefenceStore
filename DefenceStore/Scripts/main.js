function BillingTypesGraph(data) {

    var margin = { top: 20, right: 20, bottom: 70, left: 40 },
        width = 600 - margin.left - margin.right,
        height = 300 - margin.top - margin.bottom;

    var x = d3.scale.ordinal().rangeRoundBands([0, width], .05);
    var y = d3.scale.linear().range([height, 0]);

    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom");

    var yAxis = d3.svg.axis()
        .scale(y)
        .orient("left")
        .ticks(10);

    var svg = d3.select("#billing-types-graph").append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

    x.domain(data.map(function (d) { return d.BillingType; }));
    y.domain([0, d3.max(data, function (d) { return d.NumberOfOrders; })]);

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis)
        .selectAll("text")
        .attr("dx", "-.8em")
        .attr("dy", "-.55em")
        .style("text-anchor", "end")
        .attr("transform", "rotate(-90)");

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis)
        .append("text")
        .attr("y", 5)
        .attr("dy", ".71em")
        .attr("transform", "rotate(-90)")
        .style("text-anchor", "end")
        .text("NumberOfComment");

    svg.selectAll("bar")
        .data(data)
        .enter().append("rect")
        .attr("class", "bar")
        .attr("x", function (d) { return x(d.BillingType); })
        .attr("width", x.rangeBand())
        .attr("y", function (d) { return y(d.NumberOfOrders); })
        .attr("height", function (d) { return height - y(d.NumberOfOrders); });
}

function OrdersByDatesGraph(data) {
    var margin = { top: 30, right: 20, bottom: 30, left: 50 },
        width = 600 - margin.left - margin.right,
        height = 270 - margin.top - margin.bottom;

    var x = d3.time.scale().range([0, width]);
    var y = d3.scale.linear().range([height, 0]);

    var xAxis = d3.svg.axis().scale(x)
        .orient("bottom").ticks(5);

    var yAxis = d3.svg.axis().scale(y)
        .orient("left").ticks(5);

    var valueline = d3.svg.line()
        .x(function (d) { return x(d.OrderDate); })
        .y(function (d) { return y(d.NumberOfOrders); });

    var svg = d3.select("#orders-by-dates-graph")
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

    data.forEach(function (d) {
        d.OrderDate = new Date(d.OrderDate);
        d.NumberOfOrders = +d.NumberOfOrders;
    });

    x.domain(d3.extent(data, function (d) { return d.OrderDate; }));
    y.domain([0, d3.max(data, function (d) { return d.NumberOfOrders; })]);

    svg.append("path")
        .attr("class", "line")
        .attr("d", valueline(data));

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis);

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis);
}