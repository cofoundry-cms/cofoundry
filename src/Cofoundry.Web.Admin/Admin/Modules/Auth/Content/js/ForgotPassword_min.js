(function (document) {

    document.addEventListener('DOMContentLoaded', function () {
        var loginLink = document.getElementById('loginLink'),
            emailInput = document.getElementById('Username'),
            baseLoginLink = loginLink.getAttribute('href').split("?")[0];

        if (emailInput) {
            onEmailChange();

            emailInput.addEventListener('change', onEmailChange);
        }

        function onEmailChange() {
            var qs = '';

            if (emailInput.value.length) {
                qs = "?email=" + encodeURIComponent(emailInput.value);
            }
            loginLink.setAttribute('href', baseLoginLink + qs);
        }
    });

})(document);