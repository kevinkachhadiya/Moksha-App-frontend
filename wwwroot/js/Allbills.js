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
                    <div class="materialRow row g-3  align-items-center p-4 border rounded-4 bg-white shadow-sm position-relative">

        <!-- Header Section -->
        <div class="col-12 d-flex justify-content-between align-items-center mb-3">
            <h6 class="fw-bold text-primary mb-0">
                <i class="bi bi-box-seam me-2"></i>Material <span class="materialIndex">${materialIndex}</span>
            </h6>
            <button type="button" class="btn btn-sm btn-outline-danger removeMaterialBtn rounded-circle shadow-sm" title="Remove">
                <i class="bi bi-trash3"></i>
            </button>
        </div>

        <!-- Material Details Section -->
        <div class="row g-3 align-items-end">

            <!-- Material Dropdown -->
            <div class="col-md-5"> <!-- Increased to col-md-5 for more space -->
                <label class="form-label small fw-semibold text-secondary mb-2" for="MaterialId_${materialIndex}">
                    <i class="bi bi-box me-2"></i>Material
                </label>
                <select class="form-select form-select-lg shadow-sm rounded-3" id="MaterialId_${materialIndex}" name="MaterialId[]" required>
                    <option value="" selected disabled>Select Material</option>
                    <!-- Dynamic options go here -->
                </select>
            </div>

            <!-- Quantity and Total Quantity Section -->
            <div class="col-md-4">
                <label class="form-label small fw-semibold text-secondary mb-2" for="Quantity_${materialIndex}">
                    <i class="bi bi-calculator me-2"></i>Quantity
                </label>
                <div class="input-group input-group-lg shadow-sm rounded-3">
                    <!-- Quantity Input -->
                    <input type="text"
                           class="form-control quantityInput rounded-start-3"
                           id="Quantity_${materialIndex}"
                           name="Quantity[]"
                           placeholder="Qty"
                           required
                           style="font-size: 1.0rem; padding: 8px 12px; flex: 5;">

                    <!-- Equals Sign -->
                    <span class="input-group-text bg-light d-flex align-items-center justify-content-center"
                          style="font-size: 1.1rem; flex: 0.8; padding: 0 4px;">=</span>

                    <!-- Total Quantity Group -->
                    <div class="d-flex flex-column" style="flex: 2;">
                        <!-- Total Kg Label -->
                        <small class="form-text text-muted text-center mb-1" style="font-size: 0.75rem;">
                            Total Kg
                        </small>

                        <!-- Total Quantity Input -->
                        <input type="text"
                               class="form-control totalQuantity text-dark bg-light border-0 py-2"
                               id="totalQuantity_${materialIndex}"
                               name="TotalQuantity[]"
                               value="0"
                               readonly
                               style="font-size: 1.1rem; padding: 8px 10px;">
                    </div>
                </div>
            </div>

            <!-- Price Section -->
            <div class="col-md-2">
                <label class="form-label small fw-semibold text-secondary mb-2" for="Price_${materialIndex}">
                    <i class="bi bi-currency-rupee me-2"></i>Price (₹/Kg)
                </label>
                <input type="number"
                       class="form-control form-control-lg shadow-sm rounded-3"
                       id="Price_${materialIndex}"
                       name="Price[]"
                       placeholder="Price"
                       step="0.01"
                       required
                       oninput="calculateTotal(${materialIndex})">
            </div>

            <!-- Total Price Section -->
            <div class="col-md-1"> <!-- Reduced to col-md-1 for better spacing -->
                <label class="form-label small fw-semibold text-secondary mb-2" for="TotalPrice_${materialIndex}">
                    <i class="bi bi-cash-stack me-2"></i>Total
                </label>
                <input type="text"
                       class="form-control form-control-lg totalPrice shadow-sm rounded-3 bg-light"
                       id="TotalPrice_${materialIndex}"
                       name="TotalPrice[]"
                       value="0"
                       readonly>
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
            async function fetchMaterials() {
                try {
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
            const sanitized = expression.replace(/[^0-9+\-*/().]/g, "");

            // Corrected regex patterns
            if (!sanitized ||
                /^[+*\/.]/.test(sanitized) || // Hyphen removed from start check
                /[+*\/.-]{2,}/.test(sanitized) || // Hyphen moved to end
                /[+*\/.-]$/.test(sanitized) // Hyphen moved to end
            ) {
                return NaN;
            }

            const result = new Function(`return ${sanitized}`)();
            return typeof result === "number" && isFinite(result) ? result : NaN;
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

        if (!isNaN(quantity) && quantity >= 0) {
            totalQuantityField.value = quantity.toFixed(2);
            inputField.setCustomValidity("");

            const price = parseFloat(priceInput.value) || 0;
            totalPriceField.value = (quantity * price).toFixed(2);
        } else {
            totalQuantityField.value = "0.00";
            totalPriceField.value = "0.00";
            inputField.setCustomValidity("Invalid expression. Use numbers and + - * /");
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

