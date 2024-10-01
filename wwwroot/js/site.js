// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

notif = 0;
colorMode = 0;
account = 0;

//console.log("Theme: " + localStorage.getItem('theme'))

window.addEventListener('load', function () {
    if (localStorage.getItem('theme') == "Dark") {
        changeColorMode();
    }
})


function openNotif() {
    //Open Notifictions
    if (notif == 0) {
        document.getElementById("account").style.display = "none";
        document.getElementById("notifications").style.display = "block";
        notif = 1;
    }
    //Close Notifications
    else {
        document.getElementById("notifications").style.display = "none";
        notif = 0;
    } 
}

function openAccount() {
    //Open Account
    if (account == 0) {
        document.getElementById("account").style.display = "block";
        document.getElementById("notifications").style.display = "none";
        account = 1;
    }
    //Close Account
    else {
        document.getElementById("account").style.display = "none";
        account = 0;
    }
}

function changeColorMode() {
    //Dark Mode
    if (colorMode == 0) {
        document.getElementById("lightMode").style.display = "none";

        document.getElementById("darkMode").style.display = "block";
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        localStorage.setItem('theme', "Dark")
        
        colorMode = 1;
    }
    //Light Mode
    else {
        document.getElementById("lightMode").style.display = "block";
        document.documentElement.removeAttribute('data-bs-theme');
        localStorage.setItem('theme', "Light")

        document.getElementById("darkMode").style.display = "none";
        colorMode = 0;
    }
}


