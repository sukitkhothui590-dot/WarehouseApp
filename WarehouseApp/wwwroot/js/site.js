document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-dismiss-alert]").forEach((button) => {
        button.addEventListener("click", () => button.closest("[data-dismissible]")?.remove());
    });

    document.querySelectorAll(".filter-menu").forEach((menu) => {
        menu.addEventListener("click", (event) => event.stopPropagation());
    });
    document.addEventListener("click", () => {
        document.querySelectorAll(".filter-menu[open]").forEach((menu) => menu.removeAttribute("open"));
    });

    const movementPanel = document.querySelector("[data-movement-type]");
    if (!movementPanel) return;

    const movementType = movementPanel.dataset.movementType;
    const form = movementPanel.querySelector("[data-movement-form]");
    const productSelect = movementPanel.querySelector("[data-product-select]");
    const quantityInput = movementPanel.querySelector("[data-quantity-input]");
    const submitButton = movementPanel.querySelector("[data-movement-submit]");
    const previewImage = movementPanel.querySelector("[data-preview-image]");
    const previewName = movementPanel.querySelector("[data-preview-name]");
    const previewCode = movementPanel.querySelector("[data-preview-code]");
    const previewCurrent = movementPanel.querySelector("[data-preview-current]");
    const previewAfter = movementPanel.querySelector("[data-preview-after]");
    const previewMessage = movementPanel.querySelector("[data-preview-message]");

    const updateMovementPreview = () => {
        const option = productSelect?.selectedOptions[0];
        const current = Number(option?.dataset.currentQuantity);
        const quantity = Number(quantityInput?.value);
        const hasProduct = Boolean(option?.value);
        const hasQuantity = Number.isInteger(quantity) && quantity > 0;
        const isInsufficient = movementType === "withdraw" && hasProduct && hasQuantity && quantity > current;
        const canSubmit = hasProduct && hasQuantity && !isInsufficient;

        if (!option?.value) {
            previewImage.src = "/images/products/default-product.svg";
            previewName.textContent = "Select a product";
            previewCode.textContent = "Product code will appear here";
            previewCurrent.textContent = "—";
            previewAfter.textContent = "—";
            previewMessage.textContent = "Select a product and quantity to preview the new balance.";
            previewMessage.className = "preview-message";
        } else {
            const after = movementType === "receive" ? current + quantity : current - quantity;
            previewImage.src = `/images/products/${option.dataset.image}`;
            previewImage.alt = option.dataset.name || "";
            previewName.textContent = option.dataset.name || "Product";
            previewCode.textContent = `${option.dataset.code} · ${option.dataset.unit}`;
            previewCurrent.textContent = `${current} ${option.dataset.unit}`;
            previewAfter.textContent = hasQuantity ? `${after} ${option.dataset.unit}` : "—";
            previewMessage.textContent = isInsufficient ? `Insufficient stock. Only ${current} ${option.dataset.unit} available.` : hasQuantity ? "This is a preview. The balance and history will be saved together." : "Enter a quantity to preview the new balance.";
            previewMessage.className = `preview-message${isInsufficient ? " is-error" : hasQuantity ? " is-ready" : ""}`;
        }
        submitButton.disabled = !canSubmit;
    };

    productSelect?.addEventListener("change", updateMovementPreview);
    quantityInput?.addEventListener("input", updateMovementPreview);
    form?.addEventListener("submit", (event) => {
        if (submitButton.disabled) {
            event.preventDefault();
            return;
        }
        const actionText = movementType === "receive" ? "receive" : "withdraw";
        const option = productSelect.selectedOptions[0];
        if (!window.confirm(`Confirm ${actionText} ${quantityInput.value} ${option.dataset.unit} for ${option.dataset.name}?`)) event.preventDefault();
    });
    updateMovementPreview();
});
