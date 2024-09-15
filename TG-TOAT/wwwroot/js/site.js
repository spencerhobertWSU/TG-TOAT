// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

notif = 0;

function openNotif() {
    if(notif == 0) {
        document.getElementById("notifications").style.display = "block";
        notif = 1;
    }
    else {
        document.getElementById("notifications").style.display = "none";
        notif = 0;
    } 
}