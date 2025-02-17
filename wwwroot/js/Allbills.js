document.addEventListener('DOMContentLoaded', function () 
    {


            // Event listener for quantity input
            document.addEventListener("input", (event) => {
            if (event.target.classList.contains("quantityInput")) {
                updateQuantity(event.target);
            }
        });

            function updateMaterialIndices() {
                const materialRows = document.querySelectorAll('.materialRow');
                materialRows.forEach((row, index) => {
                    const materialIndexElement = row.querySelector('.materialIndex');
                    if (materialIndexElement) {
                        materialIndexElement.textContent = ((index + 1) == 1) ? 1 : (index + 1) / 2; // Sequential numbering
                    }
                });
            }
            const form = document.getElementById("addBillForm");
            const materialsContainer = document.getElementById("materialsContainer");
            const paymentMethod = document.getElementById("paymentMethod");
            const isPaidCheckbox = document.getElementById("isPaid");
            const buyerName = document.getElementById("buyerName");

          
            document.getElementById("addBillForm").addEventListener("submit", function (event) {
            event.preventDefault(); // Prevent form submission initially
            event.stopPropagation();

            let isValid = true;
            let errorMessages = [];

            // Buyer Name Validation
            const buyerName = document.getElementById("buyerName");
            const buyerNameError = document.getElementById("buyerNameError");
            const buyerNameRegex = /^[A-Za-z\s]+$/;

                if (!buyerName.value.trim() || !buyerNameRegex.test(buyerName.value)) {
                    buyerName.classList.add("is-invalid");

                    if (buyerNameError) {
                        buyerNameError.textContent = "❌ Please enter a valid buyer name (no numbers allowed).";
                        buyerNameError.style.display = "block";
                    }

                    isValid = false;
                } else {
                    buyerName.classList.remove("is-invalid");
                    buyerName.classList.add("is-valid");

                    if (buyerNameError) {
                        buyerNameError.style.display = "none";
                    }
                }


            // Materials Validation
            const materialsContainer = document.getElementById("materialsContainer");
            const materialsError = document.getElementById("materialsError");

            if (materialsContainer.children.length === 0) {
                materialsError.textContent = "❌ Please add at least one material.";
                materialsError.style.display = "block";
                isValid = false;
            } else {
                materialsError.textContent = "";
                materialsError.style.display = "none";
            }


            const isMaterialsValid = validateMaterials();
            if (!isMaterialsValid) {
                isValid = false;
            }

            // Is Paid Checkbox Validation
            const isPaidCheckbox = document.getElementById("isPaid");
            const isPaidError = document.getElementById("isPaidError");

            if (!isPaidCheckbox.checked) {
                isPaidCheckbox.classList.add("is-invalid");
                isPaidError.textContent = "💡 Please mark the bill as paid.";
                isPaidError.style.display = "block";
                isValid = false;
            } else {
                isPaidCheckbox.classList.remove("is-invalid");
                isPaidError.textContent = "";
                isPaidError.style.display = "none";
            }

            // If form is valid, submit it
            if (isValid) {
                this.submit();
            }
        });

            // Fetch materials when the modal is shown
            $('#addBillModal').on('shown.bs.modal', function () {
                fetchMaterials(); // Call async function to fetch materials
            });

            //handle add material button to pop up form model
            document.getElementById('addMaterialBtn').addEventListener('click', function () {
                const materialsContainer = document.getElementById('materialsContainer');
                const newRow = document.createElement('div');
                const materialRows = materialsContainer.children; // Get the current material rows
                let materialIndex = materialRows.length + 1; // Get the correct material index
                newRow.classList.add('materialRow', 'row', 'g-3', 'pl-4', 'ml-4', 'mb-4');
                newRow.innerHTML = `
<div class="materialRow row g-3 align-items-center p-4 border rounded-4 bg-white shadow-sm position-relative" style="transition: all 0.2s ease;">

    <!-- Header Section -->
    <div class="col-12 d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold text-primary mb-0 d-flex align-items-center">
            <i class="bi bi-box-seam me-2 fs-5"></i>Material <span class="materialIndex badge bg-primary-subtle text-primary ms-2">${materialIndex}</span>
        </h6>
        <button type="button" class="btn btn-sm btn-outline-danger removeMaterialBtn rounded-circle p-1 shadow-sm" 
                style="width: 32px; height: 32px;" title="Remove"
                onmouseover="this.style.transform='scale(1.1)'" 
                onmouseout="this.style.transform='scale(1)'">
            <i class="bi bi-trash3 fs-6"></i>
        </button>
    </div>

    <!-- Material Details Section -->
    <div class="row g-4 align-items-end">

        <!-- Material Dropdown -->
        <div class="col-lg-5">
            <div class="form-floating">
                <select class="form-select shadow-sm rounded-3 py-3" 
                        id="MaterialId_${materialIndex}" 
                        name="MaterialId[]" 
                        style="padding-top: 1.625rem; padding-bottom: 1.625rem;">
                    <option value="" selected disabled>Select Material</option>
                    <!-- Dynamic options go here -->
                </select>
                <label for="MaterialId_${materialIndex}" class="small fw-semibold text-secondary">
                    <i class="bi bi-box me-2"></i>Select Material
                </label>
            </div>
        </div>

        <!-- Quantity and Total Quantity Section -->
        <div class="col-lg-4">
            <div class="form-floating">
                <div class="input-group shadow-sm rounded-3">
                    <input type="text"
                           class="form-control quantityInput border-end-0 rounded-start-3 py-3"
                           id="Quantity_${materialIndex}"
                           name="Quantity[]"
                           placeholder="0"
                           required
                           style="padding-top: 1.625rem; padding-bottom: 1.625rem;">
                    <span class="input-group-text bg-white border-start-0 border-end-0 px-2 fw-bold text-muted">=</span>
                    <div class="form-floating flex-grow-1">
                        <input type="text"
                               class="form-control totalQuantity bg-light-subtle border-start-0 rounded-end-3 py-3"
                               id="totalQuantity_${materialIndex}"
                               name="TotalQuantity[]"
                               value="0"
                               readonly
                               style="padding-top: 1.625rem; padding-bottom: 1.625rem;">
                        <label for="totalQuantity_${materialIndex}" class="small fw-semibold text-secondary">Total (Kg)</label>
                    </div>
                </div>
            </div>
        </div>

        <!-- Price Section -->
        <div class="col-lg-2">
            <div class="form-floating">
                <input type="number"
                       class="form-control shadow-sm rounded-3 py-3"
                       id="Price_${materialIndex}"
                       name="Price[]"
                       placeholder="0.00"
                       step="0.01"
                       required
                       style="padding-top: 1.625rem; padding-bottom: 1.625rem;">
                <label for="Price_${materialIndex}" class="small fw-semibold text-secondary">
                    <i class="bi bi-currency-rupee me-2"></i>Price/Kg
                </label>
            </div>
        </div>

        <!-- Total Price Section -->
        <div class="col-lg-1">
            <div class="form-floating">
                <input type="text"
                       class="form-control totalPrice bg-light-subtle shadow-sm rounded-3 py-3"
                       id="TotalPrice_${materialIndex}"
                       name="TotalPrice[]"
                       value="0"
                       readonly
                       style="padding-top: 1.625rem; padding-bottom: 1.625rem;">
                <label for="TotalPrice_${materialIndex}" class="small fw-semibold text-secondary">
                    <i class="bi bi-cash-stack me-2"></i>Total
                </label>
            </div>
        </div>
    </div>
</div>
`;


                // Append the new row to the container
                materialsContainer.appendChild(newRow);
                populateMaterialDropdown(newRow); // Populate the dropdown with materials for the new row

                newRow.querySelector('.removeMaterialBtn').addEventListener('click', function () {
                    const materialIndexElement = newRow.querySelector('.materialIndex');
                    const currentIndex = parseInt(materialIndexElement.textContent);
                    newRow.remove();
                    updateMaterialIndices(); // Update material indices after removal
                updateGrandTotal();
                });
            });
           
            // Handle material dropdown change event
            document.getElementById('materialsContainer').addEventListener('change', function (e) {
                if (e.target && e.target.name === 'MaterialId[]') {
                    const materialSelect = e.target;
                    const newRow = materialSelect.closest('.materialRow');
                    handleMaterialChange(materialSelect, newRow);
                }
            });

            // Update total amount when price changes
            document.addEventListener('input', function (e) {
                if (e.target.matches('input[name="Price[]"]')) {
                updateGrandTotal();
                }
            });

            // Fetch materials from server
            async function fetchMaterials()

                    const response = await fetch('/Materials/all_list');
                    if (response.ok) {
                        materials = await response.json(); // Save materials globally
                        populateMaterialDropdown(); // Populate dropdown after data is available
                    } else {
                        console.error("Failed to fetch materials.", response);
                    }
                } catch (error) {
                    console.error("Error fetching materials:", error);
                }
            }

            // Populate the material dropdown
            function populateMaterialDropdown(newRow = null) {
                const materialRows = document.querySelectorAll('.materialRow');
                materialRows.forEach(row => {
                    const selectElement = row.querySelector('select[name="MaterialId[]"]');
                    if (newRow && row !== newRow) return; // Skip if this is not the newly added row

                    // Clear previous options
                    selectElement.innerHTML = '<option value="">Select Material</option>';

                    // Populate the material options
                    materials.forEach(material => {
                        const option = document.createElement('option');
                        option.value = material.id;
                        option.textContent = `${material.colorName} - ₹${material.basePrice.toFixed(2)}`;
                        selectElement.appendChild(option);
                    });
                });
            }

            // Handle material change and set base price
            function handleMaterialChange(materialSelect, newRow) {
                const selectedMaterial = materials.find(material => material.id == materialSelect.value);
                const priceInput = newRow.querySelector('input[name="Price[]"]');

                if (selectedMaterial) {
                    priceInput.value = selectedMaterial.basePrice.toFixed(2); // Set the price
                } else {
                    priceInput.value = ''; // Reset price if no material selected
                }


            updateGrandTotal(); // Recalculate the total amount
            }
});


function evaluateExpression(expression) {
    try {
        // Allow only numbers, +, -, *, /, (, and )
        const sanitized = expression.replace(/[^0-9+\-*/().]/g, "");

        // Prevent invalid sequences like "**", "//", or leading/trailing operators
        if (!/^\d|^\(|\)$/.test(sanitized) || /[*\/+.-]{2,}/.test(sanitized) || /[*\/+]$/.test(sanitized)) {
            return NaN;
        }

        // Evaluate expression safely
        const result = new Function(`return ${sanitized}`)();

        // Ensure result is a number, finite, and greater than 0
        return typeof result === "number" && isFinite(result) && result > 0 ? result : NaN;
    } catch (error) {
        return NaN;
    }
}


function updateQuantity(inputField) {
    const row = inputField.closest('.materialRow');
    const totalQuantityField = row.querySelector('.totalQuantity');
    const priceInput = row.querySelector('input[name="Price[]"]');
    const totalPriceField = row.querySelector('.totalPrice');

    const expression = inputField.value.trim();
    const quantity = expression ? evaluateExpression(expression) : 0;

    if (!isNaN(quantity) && quantity > 0) {  // Ensure quantity > 0
        totalQuantityField.value = quantity.toFixed(2);
        inputField.setCustomValidity("");

        const price = parseFloat(priceInput.value) || 0;
        totalPriceField.value = (quantity * price).toFixed(2);
    } else {
        totalQuantityField.value = "0.00";
        totalPriceField.value = "0.00";
        inputField.setCustomValidity("Invalid expression. Use numbers and + - * /, and ensure result is > 0");
    }

    updateGrandTotal();
}


    // Calculate grand total for all materials
    function updateGrandTotal() {
        let grandTotal = 0;
        document.querySelectorAll('.materialRow').forEach(row => {
            const totalPrice = parseFloat(row.querySelector('.totalPrice').value) || 0;
            grandTotal += (totalPrice)/2;
        });
        document.getElementById('totalAmount').textContent = `₹${grandTotal.toFixed(2)}`;
    }

    // Event listener for quantity inputs
    document.addEventListener("input", (event) => {
        if (event.target.classList.contains("quantityInput")) {
            updateQuantity(event.target);
        }
    });

    // Event listener for price inputs
    document.addEventListener("input", (event) => {
        if (event.target.matches('input[name="Price[]"]')) {
            updateQuantity(event.target.closest('.materialRow').querySelector('.quantityInput'));
        }
    });

    // Validate Buyer Name
    function validateBuyerName() {
        const buyerName = document.getElementById('buyerName').value.trim();
        const buyerNameError = document.getElementById('buyerNameError');
        const regex = /^[A-Za-z\s]+$/; // Only letters and spaces allowed

        if (!buyerName || !regex.test(buyerName)) {
            buyerNameError.textContent = "Please enter a valid buyer name (no numbers allowed).";
            buyerNameError.style.display = 'block'; // Show the error
            return false;
        }
        buyerNameError.textContent = "";
        return true;
    }
       
    // Validate validateMaterials
    function validateMaterials() {
        const materialRows = document.querySelectorAll('.materialRow');
        let isValid = true;

        materialRows.forEach(row => {
            const materialSelect = row.querySelector('select[name="MaterialId[]"]');
            const quantityInput = row.querySelector('input[name="Quantity[]"]');
            const priceInput = row.querySelector('input[name="Price[]"]');

            // Reset previous validation styles
            materialSelect.classList.remove("is-invalid");
            quantityInput.classList.remove("is-invalid");
            priceInput.classList.remove("is-invalid");

            // Material selection validation
            if (!materialSelect.value) {
                materialSelect.classList.add("is-invalid");
                materialSelect.style.display = 'block'; // Show the error
                isValid = false;
            }

            // Quantity validation (must be greater than 0)
            if (!quantityInput.value || isNaN(quantityInput.value) || parseFloat(quantityInput.value) <= 0) {
                quantityInput.classList.add("is-invalid");
                errorFeedback.style.display = 'block'; // Show error message
                isValid = false;
            }
            


            // Price validation (must be greater than 0)
            if (!priceInput.value || isNaN(priceInput.value) || parseFloat(priceInput.value) <= 0) {
                priceInput.classList.add("is-invalid");
                priceInput.style.display = 'block'; // Show the error
                isValid = false;
            }
        });

        return isValid;
    }

