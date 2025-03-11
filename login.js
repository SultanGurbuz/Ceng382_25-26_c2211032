//Prompt: Function to get login attempts from local storage
function getLoginAttempts() {
    return JSON.parse(localStorage.getItem('loginAttempts')) || [];
}

//Prompt: a Function to set login attempts in local storage
function setLoginAttempts(attempts) {
    localStorage.setItem('loginAttempts', JSON.stringify(attempts));
}

function incrementLoginAttempts(username, password) {
    let attempts = getLoginAttempts();
    // Check if the attempt already exists
    const exists = attempts.some(attempt => attempt.username === username && attempt.password === password);
    if (!exists) {
        attempts.push({ username, password, timestamp: new Date().toISOString() });
        setLoginAttempts(attempts);
    }
}

//Prompt: Reset login attempts
function resetLoginAttempts() {
    setLoginAttempts([]);
}

document.addEventListener('DOMContentLoaded', () => {
    const background = document.getElementById("avatarBackground");
    const benderSymbol = document.getElementById("benderSymbol");
    const loginForm = document.getElementById("loginForm");
    const symbolContainer = document.querySelector('.symbol-container');

    let videopaused = false;
    let currentSymbol = 0;
    let userLoggedIn = false;

    const symbols = [
        "images/Earth.jpeg", 
        "images/Fire.jpeg", 
        "images/Air.jpeg", 
        "images/water.jpeg"
    ];
    //Prompt: video 1 saniye oynatıldıktan sonra durdurulacak
    background.currentTime = 0;
    setTimeout(() => {
        if (!videopaused) {
            background.pause();
        }
        startSymbolRotation();
    }, 1000);

    const face = document.querySelector(".face");
    //Prompt: göz rengi beyaza değişecek üstüne mouse getirildiği zaman
    face.addEventListener("mouseover", () => {
        document.querySelectorAll(".eye").forEach(eye => {
            eye.classList.add("white-pupil");
        });
    });
    //Mouse uzaklaştığında göz rengi eski haline dönecek
    face.addEventListener("mouseout", () => {
        document.querySelectorAll(".eye").forEach(eye => {
            eye.classList.remove("white-pupil");
        });
    });
//Prompt: 1 saniyede bir eğer login olmadıysa sembol değişecek
    function startSymbolRotation() {
        setInterval(() => {
            if (!userLoggedIn) {
                changeSymbol();
            }
        }, 1000);
    }
    //Prompt:sembolün çevresine parlayan bir efekt ekle
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

    updateGlowEffect();

    function openLoginForm() {
        background.pause();
        videopaused = true;
        loginForm.classList.remove("hidden");
        loginForm.classList.add("show");
        benderSymbol.classList.add("hidden");
    }

    benderSymbol.addEventListener("click", openLoginForm);
    document.getElementById("loginButton").addEventListener("click", login);

    face.addEventListener('click', () => {
        face.classList.toggle('touch');
    });

    document.addEventListener('keydown', function(event) {
        if (event.key === 'h' || event.key === 'H') {
            const loginContainer = document.querySelector('.login-container');
            if (loginContainer) {
                loginContainer.classList.toggle('hide');
            }
        }
    });

    loginForm.addEventListener('submit', (event) => {
        event.preventDefault();
        login();
    });
    //Prompt: Login formu ekle && admin admin olursa giriş bilgileri table html sayfasına yönlendirilecek
    function login() {
        const background = document.getElementById("avatarBackground");
        const loginForm = document.getElementById("loginForm");
    
        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value.trim();
    
        // Kullanıcı adı ve şifre boş olamaz
        if (!username || !password) {
            alert("Kullanıcı adı ve şifre boş olamaz!");
            background.pause();
            return;
        }
    
        // Kullanıcı adı ve şifre doğruysa giriş yap
        if (username === "admin" && password === "admin") {
            userLoggedIn = true;
            incrementLoginAttempts(username, password); // Giriş bilgilerini kaydet
    
            loginForm.classList.remove("show");
            loginForm.classList.add("hidden");
    
            background.play();
    
            // ✅ Video bittiğinde yönlendirme yap
            background.onended = function() {
                window.location.href = "table.html"; 
            };
        } else {
            alert("Hatalı kullanıcı adı veya şifre!");
            background.pause();
        }
    }
    // Prompt: saat ekle
    function updateClock() {
        const clock = document.getElementById('clock');
        const now = new Date();
        const hours = String(now.getHours()).padStart(2, '0');
        const minutes = String(now.getMinutes()).padStart(2, '0');
        const seconds = String(now.getSeconds()).padStart(2, '0');
        clock.textContent = `${hours}:${minutes}:${seconds}`;
    }

    setInterval(updateClock, 1000);
    updateClock();
});
