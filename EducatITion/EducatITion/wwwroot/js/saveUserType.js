function saveChoice(choice) {
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "Home/SaveChoice", true);
    xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhr.send(JSON.stringify({choice: choice }));
}

function handleSave(choice) {
    saveChoice(choice);
    continueReg();
}