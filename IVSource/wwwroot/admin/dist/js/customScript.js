/**
 * Admin JS
 */
(function ($) {
    $('.nav-item a').filter(function () {
        return this.href === location.href;
    }).addClass('active');

    $('.nav left-side a').filter(function () {
        return this.href === location.href;
    }).addClass('activeNest');

})(jQuery)
