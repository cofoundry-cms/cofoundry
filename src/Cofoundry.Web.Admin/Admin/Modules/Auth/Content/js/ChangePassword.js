(function (document, window) {

    document.addEventListener('DOMContentLoaded', function () {

        /* Init */

        formatReturnUrl();

        /* Helpers */

        function formatReturnUrl() {
            var returnUrlInput = document.getElementById('ReturnUrl'),
                hash = window.location.hash;

            if (returnUrlInput.value && hash) {
                returnUrlInput.value = returnUrlInput.value + hash;
            }
        }
    });

})(document, window);