document.addEventListener("DOMContentLoaded", function () {
        // Form validation for adding materials
        const form = document.getElementById("addMaterialForm");
    if (form) {
        form.addEventListener("submit", function (event) {
            event.preventDefault();

            // Get form input values
            const colorName = document.getElementById("ColorName").value.trim();
            const basePrice = document.getElementById("BasePrice").value.trim();

            // Clear previous error messages
            const errorMessages = form.querySelectorAll(".error-message");
            errorMessages.forEach((msg) => msg.remove());

            let isValid = true;

            // Validate Color Name
            if (!colorName) {
                showError("ColorName", "Color Name is required.");
                isValid = false;
            } else if (!/^[a-zA-Z0-9\s]+$/.test(colorName)) {
                showError("ColorName", "Color Name must contain only letters and numbers.");
                isValid = false;
            } else if (colorName.length < 3) {
                showError("ColorName", "Color Name must be at least 3 characters.");
                isValid = false;
            } else if (colorName.length > 50) {
                showError("ColorName", "Color Name cannot exceed 50 characters.");
                isValid = false;
            }

            // Validate Base Price
            if (!basePrice) {
                showError("BasePrice", "Base Price is required.");
                isValid = false;
            } else if (isNaN(basePrice) || parseFloat(basePrice) <= 0) {
                showError("BasePrice", "Please enter a valid non-negative number for Base Price.");
                isValid = false;
            }

            // Submit the form if valid
            if (isValid) {
                console.log("Form submitted successfully!");
                form.submit();
            }
        });

    function showError(inputId, message) {
                const input = document.getElementById(inputId);
    const error = document.createElement("span");
    error.className = "text-danger error-message";
    error.textContent = message;

    input.classList.add("is-invalid");
    input.parentNode.appendChild(error);

    // Remove 'is-invalid' class when user starts typing
    input.addEventListener("input", function () {
        input.classList.remove("is-invalid");
    if (error.parentNode) {
        error.parentNode.removeChild(error);
                    }
                });
            }
    }

    // Alert box behavior
    const alertBox = document.getElementById('alertBox');
    if (alertBox) {
        alertBox.style.display = 'block'; // Show the alert
    alertBox.classList.add('show'); // Add class for fade-in effect

    setTimeout(function () {
        alertBox.classList.remove('show'); // Fade out effect
    setTimeout(function () {
        alertBox.style.display = 'none'; // Hide after fade-out
                }, 500);
            }, 5000); // Hide after 5 seconds
        }

        // Handle Modify button click
        document.querySelectorAll('.modify-btn').forEach(button => {
        button.addEventListener('click', function () {
            const materialId = this.getAttribute('data-id');
            fetch(`/YourController/Modify?id=${materialId}`, { method: 'GET' })
                .then(response => response.text())
                .then(data => {
                    document.querySelector('#actionModalLabel').textContent = "Modify Material";
                    document.querySelector('#actionModal .modal-body').innerHTML = data;
                    new bootstrap.Modal(document.getElementById('actionModal')).show();
                });
        });
        });

    // Handle Delete button click
    let deleteMaterialId = null; // Variable to hold the ID of the material to delete
    const deleteConfirmModalElement = document.getElementById('deleteConfirmModal');
    if (deleteConfirmModalElement) {
            const deleteConfirmModal = new bootstrap.Modal(deleteConfirmModalElement);

            document.querySelectorAll('.delete-btn').forEach(button => {
        button.addEventListener('click', function () {
            deleteMaterialId = this.getAttribute('data-id'); // Store the ID of the material
            deleteConfirmModal.show(); // Show the custom modal
        });
            });

    document.getElementById('confirmDeleteBtn').addEventListener('click', function () {
                if (deleteMaterialId) {
        fetch(`/Materials/Delete_material?id=${deleteMaterialId}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        })
            .then(response => {
                if (response.ok) {
                    console.log(`Material with ID ${deleteMaterialId} deleted successfully.`);
                    location.reload(); // Refresh the page to reflect changes
                } else {
                    // Handle non-200 HTTP responses
                    return response.json().then(errorData => {
                        console.error(`Failed to delete material. Error: ${errorData.message || 'Unknown error'}`);
                        alert(`Error: ${errorData.message || 'Unable to delete the material. Please try again.'}`);
                    });
                }
            })
            .catch(error => {
                // Handle network or other errors
                console.error('Error during delete operation:');
                alert('An error occurred while deleting the material. Please check your connection and try again.');
            });
                } else {
        console.error('No material ID provided for deletion.');
    alert('No material selected for deletion.');
                }
            });
        }
    });

    document.addEventListener("DOMContentLoaded", function () {

        // Handle Modify button click
        document.querySelectorAll('.details-btn').forEach(button => {
            button.addEventListener('click', function () {
                // Get material details from button's data attributes
                const materialId = this.getAttribute('data-id');
                const materialNo = this.getAttribute('data-material-no');
                const colorName = this.getAttribute('data-color');
                const basePrice = this.getAttribute('data-base-price');

                // Set modal fields with material data
                document.getElementById('materialNo').value = materialNo;
                document.getElementById('colorName').value = colorName;
                document.getElementById('basePrice').value = basePrice;

                // Show the modal
                const modifyModal = new bootstrap.Modal(document.getElementById('modifyModal'));
                modifyModal.show();

                // Handle form submission
                document.getElementById('modifyMaterialForm').onsubmit = function (event) {
                    event.preventDefault();

                    // Get the updated values
                    const updatedColorName = document.getElementById('colorName').value.trim();
                    const updatedBasePrice = document.getElementById('basePrice').value.trim();

                    // Clear previous error messages
                    document.querySelectorAll('.error-message').forEach(msg => msg.remove());
                    let isValid = true;

                    // Validate Color Name
                    if (!updatedColorName) {
                        showError('colorName', 'Color Name is required.');
                        isValid = false;
                    } else if (!/^[a-zA-Z0-9\s]+$/.test(updatedColorName)) {
                        showError('colorName', 'Color Name must contain only letters, numbers, and spaces.');
                        isValid = false;
                    } else if (updatedColorName.length < 3) {
                        showError('colorName', 'Color Name must be at least 3 characters.');
                        isValid = false;
                    } else if (updatedColorName.length > 50) {
                        showError('colorName', 'Color Name cannot exceed 50 characters.');
                        isValid = false;
                    }

                    // Validate Base Price
                    if (!updatedBasePrice) {
                        showError('basePrice', 'Base Price is required.');
                        isValid = false;
                    } else if (isNaN(updatedBasePrice) || updatedBasePrice < 1) {
                        showError('basePrice', 'Please enter a valid non-negative number for Base Price.');
                        isValid = false;
                    }

                    // If validation fails, stop form submission
                    if (!isValid) {
                        return;
                    }

                    // Send data via Fetch if valid
                    fetch(`/Materials/Modify/${materialId}`, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            Id: materialId, // Ensure this matches the route parameter
                            ColorName: updatedColorName,
                            BasePrice: updatedBasePrice
                        })
                    })
                        .then(response => {
                            if (response.ok) { // Check if the HTTP status is 200-299
                                if (response.status === 204) {

                                    location.reload();
                                } else {
                                    return response.json(); // Parse JSON for other status codes
                                }
                            } else {
                                throw new Error(`HTTP error! Status: ${response.status}`);
                            }
                        })
                        .then(data => {
                            if (data) {
                                // Handle response data if present
                                if (data.success) {

                                    location.reload();
                                } else {
                                    alert('Error updating material!');
                                }
                            }
                        })
                        .catch(error => {
                            console.error("Fetch Error:", error);
                            alert('Error updating material!');
                        });
                };
            });
        });

    function showError(inputId, message) {
            const input = document.getElementById(inputId);
    const error = document.createElement('span');
    error.className = 'text-danger error-message';
    error.textContent = message;
    input.parentNode.appendChild(error);

    // Remove error message on input change
    input.addEventListener('input', function () {
                if (error.parentNode) {
        error.parentNode.removeChild(error);
                }
            });
        }
    });