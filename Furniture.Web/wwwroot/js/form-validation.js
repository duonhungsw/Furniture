document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const inputs = form.querySelectorAll("input[required]");
    const submitBtn = form.querySelector("button[type='submit']");

    function validateForm() {
        let isValid = true;

        inputs.forEach(input => {
            if (input.value.trim() === "") {
                isValid = false;
            }
        });

        submitBtn.disabled = !isValid;
    }

    inputs.forEach(input => {
        input.addEventListener("input", validateForm);
    });

    validateForm();
});
