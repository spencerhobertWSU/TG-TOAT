const days = []
onlbtnChecked = 0

function dayBtn(day) {
    var campus = document.getElementById("campusSelect");
    var buildings = document.getElementById("buildSelect");
    var room = document.getElementById("roomNum");
    var startTime = document.getElementById("StartTime");
    var endTime = document.getElementById("EndTime");


    campus.disabled = false;
    //
    room.disabled = false;
    startTime.disabled = false;
    endTime.disabled = false;

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
    var startTime = document.getElementById("StartTime");
    var endTime = document.getElementById("EndTime");
    var onlBtn = document.getElementById("onlBtn")

    if (onlBtn.checked) {

        campus.disabled = true;
        buildings.disabled = true;
        room.disabled = true;
        startTime.disabled = true;
        endTime.disabled = true;

        console.log(campus.value)

        room.value = "0";
        startTime.value = '00:00'
        endTime.value = '00:00'

        

        //campus.value = "";

        for (i = 0; i < dayBtns.length; i++) {
            if (document.getElementById(dayBtns[i]).checked) {
                document.getElementById(dayBtns[i]).click();
                document.getElementById("onlBtn").click()
            }
        }
        onlbtnChecked = 1;
    }
    else {
        onlbtnChecked = 0;
        campus.disabled = false;
        room.disabled = false;
        startTime.disabled = false;
        endTime.disabled = false;
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
        option.text = "Select a Building First"
        buildings.appendChild(option);
    }
}

function UpdateInfo() {
    var campus = document.getElementById("campusSelect");
    var buildings = document.getElementById("buildSelect");
    var room = document.getElementById("roomNum");
    var startTime = document.getElementById("StartTime");
    var endTime = document.getElementById("EndTime");
    var onlBtn = document.getElementById("onlBtn")

    const dayBtns = ["monBtn", "tueBtn", "wedBtn", "thuBtn", "friBtn", "satBtn", "sunBtn"]

    days.length = 0;

    if (onlBtn.checked) {
        days.push(document.getElementById("onlBtn").value);

        campus.disabled = false;
        buildings.disabled = false;
        room.disabled = false;
        startTime.disabled = false;
        endTime.disabled = false;

        var option = document.createElement("option");
        option.value = "Onl";
        option.text = "Onl";

        campus.appendChild(option);

        var option = document.createElement("option");
        option.value = "Onl";
        option.text = "Onl";

        buildings.appendChild(option);

        campus.value = "Onl";
        buildings.value = "Onl";

        room.value = "0";
        startTime.value = '00:00'
        endTime.value = '00:00'
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

function toggleButtons(event) {

    event.preventDefault(); // Prevent form submission for the button click

    const registerBtn = document.getElementById('registerBtn');
    const dropContainer = document.getElementById('dropContainer');

    if (registerBtn) {
        // If the Register button exists, the user is not registered
        dropContainer.style.display = 'block'; // Show the Drop button
        registerBtn.style.display = 'none'; // Hide the Register button
        dropContainer.querySelector('button').form.submit(); // Submit the drop form
    } else {
        // If the Register button does not exist, the user is registered
        registerBtn.style.display = 'block'; // Show the Register button
        dropContainer.style.display = 'none'; // Hide the Drop button
        registerBtn.form.submit(); // Submit the register form

    }
}