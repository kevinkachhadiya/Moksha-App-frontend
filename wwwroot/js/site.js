
var globalAlertContainer = document.createElement("div");
    globalAlertContainer.id = "globalAlertContainer";
    globalAlertContainer.style.position = "fixed";
    globalAlertContainer.style.top = "10px";
    globalAlertContainer.style.left = "50%";
    globalAlertContainer.style.transform = "translateX(-50%)";
    globalAlertContainer.style.zIndex = "1100";  // Ensure it's above the modal
    globalAlertContainer.style.width = "400px";
    globalAlertContainer.style.maxWidth = "90%";
    globalAlertContainer.style.display = "flex";
    globalAlertContainer.style.flexDirection = "column";
    globalAlertContainer.style.alignItems = "center";


function showAlert(message, type = "danger", duration = 3000) {
    const alertDiv = document.createElement("div");
    alertDiv.classList.add("alert", `alert-${type}`, "shadow-lg", "p-3", "rounded", "animated-alert");
    alertDiv.style.position = "relative";
    alertDiv.style.marginBottom = "10px";
    alertDiv.style.opacity = "0";
    alertDiv.style.transform = "scale(0.9)";
    alertDiv.style.transition = "opacity 0.3s ease, transform 0.3s ease";
    alertDiv.style.width = "100%";
    alertDiv.style.maxWidth = "400px";
    alertDiv.style.boxShadow = "0px 4px 10px rgba(0, 0, 0, 0.1)";
    alertDiv.style.borderLeft = `5px solid ${type === "danger" ? "#dc3545" : type === "warning" ? "#ffc107" : "#28a745"}`;

    // Progress bar for auto-hide timer
    const progressBar = document.createElement("div");
    progressBar.style.height = "3px";
    progressBar.style.width = "100%";
    progressBar.style.position = "absolute";
    progressBar.style.bottom = "0";
    progressBar.style.left = "0";
    progressBar.style.backgroundColor = type === "danger" ? "#dc3545" : type === "warning" ? "#ffc107" : "#28a745";
    progressBar.style.transition = `width ${duration}ms linear`;

    alertDiv.innerHTML = `
                                <div class="d-flex justify-content-between align-items-center">
                                    <span class="fw-semibold">${message}</span>
                                    <button type="button" class="btn-close" aria-label="Close"></button>
                                </div>
                            `;

    alertDiv.appendChild(progressBar);
    globalAlertContainer.appendChild(alertDiv);

    // Show the alert with animation
    setTimeout(() => {
        alertDiv.style.opacity = "1";
        alertDiv.style.transform = "scale(1)";
        progressBar.style.width = "0"; // Progress bar animation
    }, 100);

    // Auto-hide after specified duration
    setTimeout(() => {
        alertDiv.style.opacity = "0";
        alertDiv.style.transform = "scale(0.9)";
        setTimeout(() => alertDiv.remove(), 500);
    }, duration);

    // Close button event
    alertDiv.querySelector(".btn-close").addEventListener("click", function () {
        alertDiv.style.opacity = "0";
        alertDiv.style.transform = "scale(0.9)";
        setTimeout(() => alertDiv.remove(), 500);
    });
}
   

