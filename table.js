//prompt: classname number of people description
//table: classname number of people description delete olan 
document.addEventListener("DOMContentLoaded", function () {
    const classForm = document.getElementById("classForm");
    const classTable = document.getElementById("classTable").querySelector("tbody");

    // Form Gönderildiğinde
    classForm.addEventListener("submit", function (event) {
        event.preventDefault(); // Sayfanın yenilenmesini engelle

        const className = document.getElementById("className").value.trim();
        const numPeople = document.getElementById("numPeople").value.trim();
        const description = document.getElementById("description").value.trim();

        if (!className || !numPeople || !description) {
            alert("Lütfen tüm alanları doldurun!");
            return;
        }

        // Yeni satır oluştur
        const newRow = document.createElement("tr");
        newRow.innerHTML = `
            <td>${className}</td>
            <td>${numPeople}</td>
            <td>${description}</td>
            <td><button class="delete-btn">Delete</button></td>
        `;

        // Silme butonu işlevselliği ekle
        newRow.querySelector(".delete-btn").addEventListener("click", function () {
            newRow.remove();
        });

        // Tabloya ekle
        classTable.appendChild(newRow);

        // Formu temizle
        classForm.reset();
    });
});
