// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var connection = new signalR.HubConnectionBuilder().withUrl("/monitor").build();

connection.on("ReceiveInfo", function(key, value) {
    document.getElementById("analytics").append("Received message from "+key+":"+value);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});


document.getElementById("socketbutton").addEventListener("click", function (event) {
    connection.invoke("SendInfo", "someone", "clicked this button!!!").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});