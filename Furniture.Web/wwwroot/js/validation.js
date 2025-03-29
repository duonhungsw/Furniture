document.addEventListener("DOMContentLoaded", function () {
    // Lấy phần tử trong Sign In
    const emailInput = document.getElementById("email");
    const emailError = document.getElementById("emailError");
    const hashPasswordInput = document.getElementById("hashPassword");
    const hashPasswordError = document.getElementById("hashPasswordError");
    const signInBtn = document.getElementById("signInBtn");

    // Lấy phần tử trong Sign Up
    const userNameInput = document.getElementById("username");
    const userNameError = document.getElementById("userNameError");
    const emailSignUpInput = document.getElementById("emailSignUp");
    const emailSignUpError = document.getElementById("emailSignUpError");
    const passwordInput = document.getElementById("password");
    const passwordError = document.getElementById("passwordError");
    const confirmPasswordInput = document.getElementById("confirmPassword");
    const confirmPasswordError = document.getElementById("confirmPasswordError");
    const signUpBtn = document.getElementById("signUpBtn");

    // Lấy form quên mật khẩu
    const forgotPasswordForm = document.getElementById("forgotPasswordForm");

    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    function validateSignInForm() {
        let isValid = true;

        if (!emailPattern.test(emailInput.value)) {
            emailError.textContent = "Invalid email format (e.g., example@gmail.com)";
            emailError.style.color = "red";
            isValid = false;
        } else {
            emailError.textContent = "";
        }

        if (hashPasswordInput.value.length < 6) {
            hashPasswordError.textContent = "Password must be at least 6 characters";
            hashPasswordError.style.color = "red";
            isValid = false;
        } else {
            hashPasswordError.textContent = "";
        }

        signInBtn.disabled = !isValid;
    }

    function validateSignUpForm() {
        let isValid = true;

        if (userNameInput.value.length < 6) {
            userNameError.textContent = "UserName must be at least 6 characters";
            userNameError.style.color = "red";
            isValid = false;
        } else {
            userNameError.textContent = "";
        }

        if (!emailPattern.test(emailSignUpInput.value)) {
            emailSignUpError.textContent = "Invalid email format (e.g., example@gmail.com)";
            emailSignUpError.style.color = "red";
            isValid = false;
        } else {
            emailSignUpError.textContent = "";
        }

        if (passwordInput.value.length < 6) {
            passwordError.textContent = "Password must be at least 6 characters";
            passwordError.style.color = "red";
            isValid = false;
        } else {
            passwordError.textContent = "";
        }

        if (confirmPasswordInput.value !== passwordInput.value) {
            confirmPasswordError.textContent = "Passwords do not match";
            confirmPasswordError.style.color = "red";
            isValid = false;
        } else {
            confirmPasswordError.textContent = "";
        }

        signUpBtn.disabled = !isValid;
    }

    // Thêm sự kiện để kiểm tra khi nhập liệu
    if (emailInput && hashPasswordInput) {
        emailInput.addEventListener("input", validateSignInForm);
        hashPasswordInput.addEventListener("input", validateSignInForm);
    }

    if (userNameInput && emailSignUpInput && passwordInput && confirmPasswordInput) {
        userNameInput.addEventListener("input", validateSignUpForm);
        emailSignUpInput.addEventListener("input", validateSignUpForm);
        passwordInput.addEventListener("input", validateSignUpForm);
        confirmPasswordInput.addEventListener("input", validateSignUpForm);
    }

    // Ẩn form Forgot Password khi có thông báo
    const successMessage = "@(TempData["Success"] ?? "")";
    const errorMessage = "@(TempData["Error"] ?? "")";

    if ((successMessage && successMessage !== "") || (errorMessage && errorMessage !== "")) {
        if (forgotPasswordForm) {
            forgotPasswordForm.style.display = "none"; // Ẩn form
            setTimeout(function () {
                forgotPasswordForm.style.display = "block"; // Hiện lại form sau 3 giây
            }, 3000);
        }
    }
});
