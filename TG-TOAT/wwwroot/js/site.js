colorMode = 0;

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
    // Toggle Notifications
    var notifMenu = document.getElementById("notificationDropdown");
    var accountMenu = document.getElementById("account")

    if (notifMenu.style.display === "none" || notifMenu.style.display === "") {
        // Show the notification dropdown
        notifMenu.style.display = "block";
        accountMenu.style.display = "none";
    } else {
        // Hide the notification dropdown
        notifMenu.style.display = "none";

    }
    
}

function openAccount() {
    var accountMenu = document.getElementById("account")
    var notifMenu = document.getElementById("notifications")

    //Open Account
    if (accountMenu.style.display == "none") {
        accountMenu.style.display = "block";
        if (notifMenu != null) {
            notifMenu.style.display = "none";
        }
        
    }
    //Close Account
    else {
        accountMenu.style.display = "none";
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


