(function ($) {

    // Context
    var $el = $(document.getElementsByClassName('main-nav'));

    // Temp vars
    var categories, currentCategory;

    function init() {

        categories = $(document.getElementsByClassName('category'));
        currentCategory = $(document.getElementsByClassName('category selected'));

        //Events
        categories.on('mouseenter', function (e) {
            var $src = $(e.srcElement);
            
            currentCategory.removeClass('selected');
            //$src.addClass('selected');
        });

        categories.on('mouseleave', function (e) {
            var $src = $(e.srcElement);

            currentCategory.addClass('selected');
            //$src.removeClass('selected');
        });
    }

    init();

})(angular.element);