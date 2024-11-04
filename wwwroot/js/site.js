notif = 0;
colorMode = 0;
account = 0;

if (localStorage.getItem('theme') == "Dark") {
    changeColorMode();
}

//console.log("Theme: " + localStorage.getItem('theme'))

function navIndicator() {
    let url = document.URL.split('/');
    currPage = "nav" + url[url.length - 1];

    pageId = document.getElementById(currPage)
    if (pageId != null) {
        pageId.classList.add("currPage")
    }
    
}

window.addEventListener('load', function () {
    classCards = document.getElementsByClassName("card");


})

function openNotif() {
    //Open Notifictions
    var accountMenu = document.getElementById("account")
    var notifMenu = document.getElementById("notifications")

    if (notif == 0) {
        accountMenu.style.display = "none";
        notifMenu.style.display = "block";
        notif = 1;
    }
    //Close Notifications
    else {
        notifMenu.style.display = "none";
        notif = 0;
    } 

}

function openAccount() {
    var accountMenu = document.getElementById("account")
    var notifMenu = document.getElementById("notifications")

    //Open Account
    if (account == 0) {
        accountMenu.style.display = "block";
        notifMenu.style.display = "none";
        account = 1;
    }
    //Close Account
    else {
        notifMenu.style.display = "none";
        account = 0;
    }

}

function changeColorMode() {
    //Dark Mode
    if (colorMode == 0) {
        document.getElementById("darkMode").style.display = "none";
        document.getElementById("lightMode").style.display = "block";

        document.documentElement.setAttribute('data-bs-theme', 'dark');
        localStorage.setItem('theme', "Dark")
        
        colorMode = 1;
    }
    //Light Mode
    else {

        document.getElementById("lightMode").style.display = "none";
        document.getElementById("darkMode").style.display = "block";

        document.documentElement.removeAttribute('data-bs-theme');
        localStorage.setItem('theme', "Light")


        colorMode = 0;
    }
}

function classInfo(info) {
    var modal = document.getElementById("myModal");
    var span = document.getElementsByClassName("close")[0];
    var text = document.getElementById("modalText");

    text.innerHTML = info;

    modal.style.display = "block";

    span.onclick = function () {
        modal.style.display = "none";
    }

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    }

}


