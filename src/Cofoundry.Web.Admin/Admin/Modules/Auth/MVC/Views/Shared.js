(function (document, window) {

    document
        .getElementById('MainForm')
        .addEventListener('submit', disableButton, false);

    formatReturnUrl();

    function disableButton() {
        this.querySelector('input[type="submit"]')
            .setAttribute('disabled', 'disabled');
    }

    function formatReturnUrl() {
        var returnUrlInput = document.getElementById('ReturnUrl'),
            hash = window.location.hash;

        if (returnUrlInput.value && hash) {
            returnUrlInput.value = returnUrlInput.value + hash;
        }
    }
})(document, window);