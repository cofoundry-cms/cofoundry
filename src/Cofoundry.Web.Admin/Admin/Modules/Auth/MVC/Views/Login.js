(function (document, window) {

    document.addEventListener('DOMContentLoaded', function () {
        var forgotPasswordLink = document.getElementById('forgotPasswordLink'),
            emailInput = document.getElementById('EmailAddress'),
            baseLink = forgotPasswordLink.getAttribute('href');

        /* Init */

        onEmailChange();
        formatReturnUrl();
        emailInput.addEventListener('change', onEmailChange);

        /* Helpers */

        function onEmailChange() {
            var qs;

            if (emailInput.value.length) {
                qs = "?email=" + encodeURIComponent(emailInput.value);
            }

            forgotPasswordLink.setAttribute('href', baseLink + qs);
        }

        function formatReturnUrl() {
            var returnUrlInput = document.getElementById('ReturnUrl'),
                hash = window.location.hash;

            if (returnUrlInput.value && hash) {
                returnUrlInput.value = returnUrlInput.value + hash;
            }
        }
    });

})(document, window);