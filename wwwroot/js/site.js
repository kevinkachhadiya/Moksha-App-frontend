function initAlertSystem() {
    // Check if container already exists
    if (document.getElementById('globalAlertContainer')) {
        return;
    }

    // Create container
    var globalAlertContainer = document.createElement("div");
    globalAlertContainer.id = "globalAlertContainer";
    globalAlertContainer.style.position = "fixed";
    globalAlertContainer.style.top = "10px";
    globalAlertContainer.style.left = "50%";
    globalAlertContainer.style.transform = "translateX(-50%)";
    globalAlertContainer.style.zIndex = "1100";
    globalAlertContainer.style.width = "400px";
    globalAlertContainer.style.maxWidth = "90%";
    globalAlertContainer.style.display = "flex";
    globalAlertContainer.style.flexDirection = "column";
    globalAlertContainer.style.alignItems = "center";
    globalAlertContainer.setAttribute('aria-live', 'polite');

    // Append only if body exists
    if (document.body) {
        document.body.appendChild(globalAlertContainer);
    } else {
        console.error("Could not initialize alert system: document.body not found");
    }
}

// Initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initAlertSystem);
} else {
    initAlertSystem(); // DOM already loaded
}
/**
 * Show an alert notification
 * @param {string} message - The message to display
 * @param {string} type - Alert type: 'success', 'warning', or 'danger' (default: 'danger')
 * @param {number} duration - Time in ms before auto-dismiss (default: 3000)
 */
function showAlert(message, type = "danger", duration = 3000) {
    // Handle empty or invalid messages
    if (!message || typeof message !== 'string') {
        message = "Notification";
        console.warn("showAlert called with invalid message");
    }

    // Create alert element
    const alertDiv = document.createElement("div");
    alertDiv.classList.add("alert", `alert-${type}`, "shadow-lg", "p-3", "rounded", "animated-alert");

    // Set modern styling
    Object.assign(alertDiv.style, {
        position: "relative",
        marginBottom: "10px",
        opacity: "0",
        transform: "scale(0.9)",
        transition: "opacity 0.3s ease, transform 0.3s ease",
        width: "100%",
        maxWidth: "400px",
        boxShadow: "0px 4px 10px rgba(0, 0, 0, 0.1)",
        display: "flex",
        flexDirection: "column",
        overflow: "hidden" // For progress bar
    });

    // Set color schemes based on alert type
    const colors = {
        danger: { border: "#dc3545", bg: "#f8d7da", text: "#721c24" },
        warning: { border: "#ffc107", bg: "#fff3cd", text: "#856404" },
        success: { border: "#28a745", bg: "#d4edda", text: "#155724" },
        info: { border: "#17a2b8", bg: "#d1ecf1", text: "#0c5460" }
    };

    // Apply color scheme (with fallback to danger)
    const scheme = colors[type] || colors.danger;
    alertDiv.style.borderLeft = `5px solid ${scheme.border}`;
    alertDiv.style.backgroundColor = scheme.bg;
    alertDiv.style.color = scheme.text;

    // Create appropriate icon based on alert type
    let iconSvg = '';
    switch (type) {
        case 'success':
            iconSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"/>
            </svg>`;
            break;
        case 'warning':
            iconSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
                <path d="M7.002 11a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 4.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 4.995z"/>
            </svg>`;
            break;
        case 'info':
            iconSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z"/>
            </svg>`;
            break;
        default: // danger
            iconSvg = `<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
            </svg>`;
    }

    // Create alert content with icon and message
    alertDiv.innerHTML = `
        <div class="d-flex justify-content-between align-items-center" style="width: 100%;">
            <div class="d-flex align-items-center">
                <span style="margin-right: 8px;">${iconSvg}</span>
                <span class="fw-semibold" style="word-break: break-word;">${message}</span>
            </div>
            <button type="button" class="btn-close" style="background: none; border: none; cursor: pointer; padding: 0; opacity: 0.5;" aria-label="Close">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z"/>
                </svg>
            </button>
        </div>
    `;

    // Create progress bar
    const progressBar = document.createElement("div");
    progressBar.style.height = "3px";
    progressBar.style.width = "100%";
    progressBar.style.position = "absolute";
    progressBar.style.bottom = "0";
    progressBar.style.left = "0";
    progressBar.style.backgroundColor = scheme.border;
    progressBar.style.transition = `width ${duration}ms linear`;
    alertDiv.appendChild(progressBar);

    // Add the alert to the container
    globalAlertContainer.appendChild(alertDiv);

    // Make sure container is in the DOM
    if (!globalAlertContainer.parentNode) {
        document.body.appendChild(globalAlertContainer);
    }

    // Show the alert with animation (using requestAnimationFrame for better performance)
    requestAnimationFrame(() => {
        setTimeout(() => {
            alertDiv.style.opacity = "1";
            alertDiv.style.transform = "scale(1)";
            // Start progress bar animation
            void progressBar.offsetWidth; // Force reflow to ensure animation works
            progressBar.style.width = "0";
        }, 10);
    });

    // Auto-hide after specified duration
    const dismissTimeout = setTimeout(() => {
        dismissAlert(alertDiv);
    }, duration);

    // Close button event
    const closeButton = alertDiv.querySelector(".btn-close");
    if (closeButton) {
        closeButton.addEventListener("click", function () {
            clearTimeout(dismissTimeout); // Clear the auto-dismiss timeout
            dismissAlert(alertDiv);
        });
    }

    // Helper function to dismiss with animation
    function dismissAlert(element) {
        if (!element || !element.parentNode) return;

        element.style.opacity = "0";
        element.style.transform = "scale(0.9)";

        setTimeout(() => {
            if (element && element.parentNode) {
                element.parentNode.removeChild(element);
            }
        }, 300);
    }

    // Return the alert element (could be useful for references)
    return alertDiv;
}

// Add this function to check if container is in DOM and append if needed
function ensureContainerInDOM() {
    if (!document.getElementById('globalAlertContainer')) {
        document.body.appendChild(globalAlertContainer);
    }
}

// Call this when document is ready
if (document.readyState === 'complete') {
    ensureContainerInDOM();
} else {
    window.addEventListener('load', ensureContainerInDOM);
}