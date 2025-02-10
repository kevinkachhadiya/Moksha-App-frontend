document.addEventListener("DOMContentLoaded", function () {
    let existingModal = document.getElementById("uniqueLoginModal");
    if (existingModal) {
        existingModal.remove(); // Remove duplicate modals before appending a new one
    }

    // Check if AuthToken exists in cookies
    var token = getCookie("AuthToken");
    let loginModalElement = document.getElementById("uniqueLoginModal");

     if (loginModalElement) {
        let loginModal = new bootstrap.Modal(loginModalElement);
        
        var token = getCookie("AuthToken");
        if (!token) {
            loginModal.show();
        } else {
            validateToken(token).then(isValid => {
                if (!isValid) {
                    loginModal.show();
                }
            });
        }
    } else {
        console.warn("Login modal not found. Check if _LoginPartial is correctly loaded.");
    }
});

// Function to validate the token by making an API request
async function validateToken(token) {
    try {
        const BaseUrl = "/Auth/ValidateToken";

        let response = await fetch(BaseUrl, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,  // Add the token to the request headers if needed
            },
        });

        if (response.ok) {
            return true;
        } else {
            // Token is invalid or expired
            return false;
        }
    } catch (error) {
        console.error("Error during token validation:", error);
        return false; // If any error occurs, assume token is invalid
    }
}

// Function to get cookie value by name
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(nameEQ) == 0) {
            return c.substring(nameEQ.length, c.length); // Return the value of the cookie
        }
    }
    return null; // Return null if cookie is not found
}