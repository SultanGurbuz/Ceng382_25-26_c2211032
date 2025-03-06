// Prompt:  Sayfa yüklendiğinde çalışacak bir fonksiyon yaz.
document.addEventListener('DOMContentLoaded', () => {
    const background = document.getElementById("avatarBackground");
    const benderSymbol = document.getElementById("benderSymbol");
    const loginForm = document.getElementById("loginForm");
    const symbolContainer = document.querySelector('.symbol-container');
    
    let videopaused = false;
    let currentSymbol = 0;
    let userLoggedIn = false;
    //prompt:4 element sembolü döngü şeklinde değişsin
    const symbols = [
        "images/Earth.jpeg", 
        "images/Fire.jpeg", 
        "images/Air.jpeg", 
        "images/water.jpeg"
    ];
    //prompt: video 1 saniye sonra durdurulup sembol döngüsü başlasın.
    background.currentTime = 0;
    setTimeout(() => {
        if (!videopaused) {
            background.pause();
        }
        startSymbolRotation();
    }, 1000);
    const face = document.querySelector(".face");
    const eyes = document.querySelectorAll(".eye::after"); // Pseudo-element doğrudan seçilemez!

    face.addEventListener("mouseover", () => {
    document.querySelectorAll(".eye").forEach(eye => {
        eye.classList.add("white-pupil");
    });
    });

    face.addEventListener("mouseout", () => {
        document.querySelectorAll(".eye").forEach(eye => {
        eye.classList.remove("white-pupil");
        });
    });

    // Sembol değişimi login olana kadar devam etsin
    function startSymbolRotation() {
        setInterval(() => {
            if (!userLoggedIn) {
                changeSymbol();
            }
        }, 1000);
    }

  
    function changeSymbol() {
        currentSymbol = (currentSymbol + 1) % symbols.length;
        benderSymbol.src = symbols[currentSymbol];
        updateGlowEffect();
    }

    function updateGlowEffect() {
        symbolContainer.classList.remove('earth', 'fire', 'air', 'water');
        switch (currentSymbol) {
            case 0:
                symbolContainer.classList.add('earth');
                break;
            case 1:
                symbolContainer.classList.add('fire');
                break;
            case 2:
                symbolContainer.classList.add('air');
                break;
            case 3:
                symbolContainer.classList.add('water');
                break;
        }
    }

    // Initialize the glow effect
    updateGlowEffect();

    // Login formunu aç
    function openLoginForm() {
      background.pause();
      videopaused = true;
      loginForm.classList.remove("hidden");
      loginForm.classList.add("show");
      benderSymbol.classList.add("hidden");
    }

    
    benderSymbol.addEventListener("click", openLoginForm);
    document.getElementById("loginButton").addEventListener("click", login);
    //kaş çatma efekti

    face.addEventListener('click', () => {
        face.classList.toggle('touch');
    });

    function login() {
      background.play();
      
      const username = document.getElementById("username").value.trim();
      const password = document.getElementById("password").value.trim();

      if (username && password) {
        // Login formu gizle
        const loginArray = [username, password];
        console.log(loginArray);  
        const loginForm = document.getElementById("loginForm");
        loginForm.classList.remove("show");
        loginForm.classList.add("hidden");

        // videoyu tekrar oynat
        const background = document.getElementById("avatarBackground");
        background.play();

        // Sembol döngüsünü durdur
        userLoggedIn = true;
      } else {
        background.pause();
        userLoggedIn = false;
        alert("Kullanıcı adı ve şifre boş olamaz!");
        videopaused = true;
      }
    }
});