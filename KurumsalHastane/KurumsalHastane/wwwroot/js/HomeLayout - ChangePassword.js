
var chLetter = document.getElementById("chLetter");
var chCapital = document.getElementById("chCapital");
var chNumber = document.getElementById("chNumber");
var chLength = document.getElementById("chLength");
var notEqual = document.getElementById("chNotEqual");



// change Password
var password = document.getElementById("chPassword")
    , confirm_password = document.getElementById("confirm_password");

function validatePassword() {
    if (password.value != confirm_password.value) {
        notEqual.classList.remove("valid");
        notEqual.classList.add("invalid");
    } else {
        notEqual.classList.remove("invalid");
        notEqual.classList.add("valid");
    }
}

// When the user clicks on the password field, show the message box
password.onfocus = function () {
    document.getElementById("chMessage").style.display = "block";
    chLetter.style.display = "";
    chCapital.style.display = "";
    chNumber.style.display = "";
    chLength.style.display = "";
    notEqual.style.display = "";
}
confirm_password.onfocus = function () {
    document.getElementById("chMessage").style.display = "block";
    chLetter.style.display = "none";
    chCapital.style.display = "none";
    chNumber.style.display = "none";
    chLength.style.display = "none";
    notEqual.style.display = "";
}
// When the user clicks outside of the password field, hide the message box
password.onblur = function () {
    document.getElementById("chMessage").style.display = "none";
   chLetter.style.display = "none";
    chCapital.style.display = "none";
    chNumber.style.display = "none";
    chLength.style.display = "none";
    notEqual.style.display = "none";
}
confirm_password.onblur = function () {
    notEqual.style.display = "none";
    document.getElementById("chMessage").style.display = "none";
}
// When the user starts to type something inside the password field
password.onkeyup = function () {
    // Validate lowercase letters
    var lowerCaseLetters = /[a-z]/g;
    if (password.value.match(lowerCaseLetters)) {
        chLetter.classList.remove("invalid");
        chLetter.classList.add("valid");
    } else {
        chLetter.classList.remove("valid");
        chLetter.classList.add("invalid");
    }

    // Validate chCapital letters
    var upperCaseLetters = /[A-Z]/g;
    if (password.value.match(upperCaseLetters)) {
        chCapital.classList.remove("invalid");
        chCapital.classList.add("valid");
    } else {
        chCapital.classList.remove("valid");
        chCapital.classList.add("invalid");
    }

    // Validate numbers
    var numbers = /[0-9]/g;
    if (password.value.match(numbers)) {
        chNumber.classList.remove("invalid");
        chNumber.classList.add("valid");
    } else {
        chNumber.classList.remove("valid");
        chNumber.classList.add("invalid");
    }

    // Validate chLength
    if (password.value.length >= 8) {
        chLength.classList.remove("invalid");
        chLength.classList.add("valid");
    } else {
        chLength.classList.remove("valid");
        chLength.classList.add("invalid");
    }

} 

password.onchange = validatePassword;
confirm_password.onkeyup = validatePassword;