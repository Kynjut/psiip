function switchForms(element) {
    if (element.className == 'auth-reg-ref') {
        document.getElementById('enter-close-cross').click();
        document.getElementById('reg-form-open-btn').click();
    }
    else {
        document.getElementById('reg-close-cross').click();
        document.getElementById('auth-form-open-btn').click();
    }
}

function continueReg() {
    document.getElementById('role-choose-close-cross').click();
    document.getElementById('reg-continue-open-btn').click();
}