function editAdd() {
    // Retrieve form inputs
    let inputs = document.querySelectorAll('input[type="text"]');
    let editbtn = document.getElementById('AddEdit');
    let submitButton = document.getElementById('AddSave');

    add1 = document.getElementById("addressLineOne").value;
    add2 = document.getElementById("addressLineTwo").value;
    zip = document.getElementById("zip").value;

    if (editbtn.innerText === 'Edit') {
        // Enable editing
        inputs.forEach(input => input.removeAttribute('readonly'));
        editbtn.innerText = 'Cancel';
        submitButton.style.display = 'inline'; // Show the submit button
    }
    else {
        inputs.forEach(input => input.setAttribute('readonly', 'readonly'));
        editbtn.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

        document.getElementById("addressLineOne").value = add1;
        document.getElementById("addressLineTwo").value = add2;
        document.getElementById("zip").value = zip;
        
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
        inputs.forEach(input => input.setAttribute('readonly', 'readonly'));
        showToast();
        button.innerText = 'Edit';
        submitButton.style.display = 'none'; // Hide the submit button

        // Submit the form
        document.querySelector('form').submit();
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