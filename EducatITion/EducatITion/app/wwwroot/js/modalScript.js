var modals = document.querySelectorAll(".modal");
var btns = document.querySelectorAll(".openModal");
var closes = document.querySelectorAll(".closeModal");

for (let i = 0; i < modals.length; i++) {
    if (btns != null && btns[i] != null) {
        btns[i].onclick = function () {
            var width = modals[i].getAttribute("data-width");
            var height = modals[i].getAttribute("data-height");

            modals[i].querySelector(".modal-content").style.width = width;
            modals[i].querySelector(".modal-content").style.height = height;

            modals[i].style.display = "block";
        }
    }

    if (closes != null && closes[i] != null) {
        closes[i].onclick = function () {
            modals[i].style.display = "none";
        }
    }
}

window.onclick = function (event) {
    modals.forEach(function (modal) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    })
}