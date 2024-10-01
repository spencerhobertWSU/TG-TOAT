const days = []

function dayBtn(day) {
    var campus = document.getElementById("campusSelect");
    var buildings = document.getElementById("buildSelect");
    var room = document.getElementById("roomNum");


    campus.disabled = false;
    buildings.disabled = false;
    room.disabled = false;

    if (document.getElementById("onlBtn").checked) {
        document.getElementById("onlBtn").click();
        document.getElementById(day).click();
    }
}

function onlineBtn() {
    const dayBtns = ["monBtn", "tueBtn", "wedBtn", "thuBtn", "friBtn", "satBtn", "sunBtn"]
    var campus = document.getElementById("campusSelect");
    var buildings = document.getElementById("buildSelect");
    var room = document.getElementById("roomNum");

    campus.disabled = true;
    buildings.disabled = true;
    room.disabled = true;

    for (i = 0; i < dayBtns.length; i++) {
        if (document.getElementById(dayBtns[i]).checked) {
            document.getElementById(dayBtns[i]).click();
            document.getElementById("onlBtn").click()
        }
    }
}

const ogdenValues = ["", "BC", "EH", "ET", "HC", "IE", "KA", "LP", "LL", "LH", "MH", "ED", "NB", "HB", "SU", "LI", "SC", "SW", "TY", "WB"]
const ogdenNames =  ["Select a Building","Browing Center", "Elizabeth Hall", "Engineering Technology", "Hurst Center For Lifelong Learning", "Interprofessional Education Building",
                    "Kimball Visual Arts Center", "Lampros Hall", "Lind Lecture Hall", "Lindquist Hall", "Marriott Health Services", "McKay Education",
                    "Noorda Engineering, Applied Science & Technology", "Noorda High Bay", "Shepherd Union", "Steward Library", "Student Services Center", "Swenson Building",
                    "Tracey Hall Science Center", "Wattis Building"]

const laytonValues = ["", "D2", "D13", "DSC", "CCE", "CAE"]
const laytonNames = ["Select a Building", "Building D2", "Building D13", "Stewart Center", "Center for Continuing Education", "Computer & Automotive Engineering"]


function changeCampus() {
    campus = (document.getElementById("campusSelect").value)

    var buildings = document.getElementById("buildSelect");
    console.log();

    if (campus == "Ogden") {
        buildings.disabled = false;
        buildings.innerHTML = '';
        for (i = 0; i < ogdenValues.length; i++) {
            var option = document.createElement("option");
            option.value = ogdenValues[i];
            option.text = ogdenNames[i];
            buildings.appendChild(option);
        }
    }
    else if (campus == "Davis") {
        buildings.disabled = false;
        buildings.innerHTML = '';
        for (i = 0; i < laytonValues.length; i++) {
            var option = document.createElement("option");
            option.value = laytonValues[i];
            option.text = laytonNames[i];
            buildings.appendChild(option);
        }
    }
    else {
        buildings.disabled = true;
        buildings.innerHTML = '';
        var option = document.createElement("option");
        option.value = ""
        option.text = "Select a Campus First"
        buildings.appendChild(option);
    }
}

function UpdateDays() {
    const dayBtns = ["monBtn", "tueBtn", "wedBtn", "thuBtn", "friBtn", "satBtn", "sunBtn"]

    days.length = 0;

    if (document.getElementById("onlBtn").checked) {
        days.push(document.getElementById("onlBtn").value);
    }
    else {
        for (i = 0; i < dayBtns.length; i++) {
            if (document.getElementById(dayBtns[i]).checked) {
                days.push(document.getElementById(dayBtns[i]).value);
            }
        }
    }
    document.getElementById("DaysOfTheWeek").value = days;
}