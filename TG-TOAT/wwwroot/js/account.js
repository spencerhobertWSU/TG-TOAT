const stateValues = ['',
    'AL', 'AK', 'AZ', 'AR', 'CA', 'CO', 'CT', 'DE', 'FL', 'GA',
    'HI', 'ID', 'IL', 'IN', 'IA', 'KS', 'KY', 'LA', 'ME', 'MD',
    'MA', 'MI', 'MN', 'MS', 'MO', 'MT', 'NE', 'NV', 'NH', 'NJ',
    'NM', 'NY', 'NC', 'ND', 'OH', 'OK', 'OR', 'PA', 'RI', 'SC',
    'SD', 'TN', 'TX', 'UT', 'VT', 'VA', 'WA', 'WV', 'WI', 'WY']
const stateNames = ["Select a State",
    "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "Florida", "Georgia",
    "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland",
    "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey",
    "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina",
    "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"]

function editAdd() {
    // Retrieve form inputs
    let inputs = document.querySelectorAll('.userAdd');
    let editbtn = document.getElementById('AddEdit');
    let submitButton = document.getElementById('AddSave');

    add1 = document.getElementById("addressLineOne").value;
    add2 = document.getElementById("addressLineTwo").value;
    zip = document.getElementById("zip").value;

    if (editbtn.innerText === 'Edit') {
        // Enable editing
        updateStates()
        inputs.forEach(input => input.removeAttribute('disabled'));
        editbtn.innerText = 'Cancel';
        submitButton.style.display = 'inline'; // Show the submit button
    }
    else {
        inputs.forEach(input => input.setAttribute('disabled', 'disabled'));
        editbtn.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

        document.getElementById("addressLineOne").value = add1;
        document.getElementById("addressLineTwo").value = add2;
        document.getElementById("zip").value = zip;
        
    }
}

function editName() {
    // Retrieve form inputs
    let inputs = document.querySelectorAll('.userName');
    let editbtn = document.getElementById('NameEdit');
    let submitButton = document.getElementById('NameSave');

    if (editbtn.innerText === 'Edit') {

        firstName = document.getElementById("firstName").value;
        lastName = document.getElementById("lastName").value;
        // Enable editing
        updateStates()
        inputs.forEach(input => input.removeAttribute('disabled'));
        editbtn.innerText = 'Cancel';
        submitButton.style.display = 'inline'; // Show the submit button
    }
    else {
        inputs.forEach(input => input.setAttribute('disabled', 'disabled'));
        editbtn.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

        document.getElementById("firstName").value = firstName;
        document.getElementById("lastName").value = lastName;

    }
}

function saveAdd() {
    // Retrieve form inputs
    let inputs = document.querySelectorAll('input[type="text"]');
    let editbtn = document.getElementById('AddEdit');
    let submitButton = document.getElementById('AddSave');

    let confirmSave = confirm("Are you sure you want to save the changes?");
    if (confirmSave) {
        // Disable editing
        //inputs.forEach(input => input.setAttribute('readonly', 'readonly'));
        showToast();
        button.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

        // Submit the form
        document.querySelector('form').submit();
    }
}

function saveName() {
    // Retrieve form inputs
    let inputs = document.querySelectorAll('input[type="text"]');
    let editbtn = document.getElementById('AddEdit');
    let submitButton = document.getElementById('NameSave');

    let confirmSave = confirm("Are you sure you want to save the changes?");
    if (confirmSave) {
        // Disable editing
        //inputs.forEach(input => input.setAttribute('disabled', 'disabled'));
        showToast();
        button.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

    }
}

// Success toast notification
function showToast() {
    let toast = document.getElementById('toast');
    toast.style.display = 'block';
    setTimeout(() => {
        toast.style.display = 'none';
    }, 1000);
}
