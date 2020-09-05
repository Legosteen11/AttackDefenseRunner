// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var connection = new signalR.HubConnectionBuilder().withUrl("/monitor").build();

connection.on("ServiceRunner", function(running) {
    if (running) {
        document.getElementById("startservice").hidden = true;
        document.getElementById("stopservice").hidden = false;

    } else {
        document.getElementById("startservice").hidden = false;
        document.getElementById("stopservice").hidden = true;
    }
});

connection.on("ConfigUpdate", function(key, value) {
    if (document.getElementById(key) != null) {
        document.getElementById(key).value = value;
    }
});


connection.start().then(function () {
    //Nothing yet
}).catch(function (err) {
    return console.error(err.toString());
});


// connection.invoke("SendInfo", "someone", "clicked this button!!!").catch(function (err) {
//     return console.error(err.toString());
// });
