var connection = new signalR.HubConnectionBuilder().withUrl("/LCMVCHUB").build();

connection.start().then(function () {
    console.log("Connected to SignalR hub.");
}).catch(function (err) {
    console.error(err.toString());
});