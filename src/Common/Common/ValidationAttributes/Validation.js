/* File Created: August 31, 2012 */
/* Project needs jquery.validate and jquery.validate.unobtrusive for these to work*/
(function ($) {
    //validation for RequiredIfAttribute
    $.validator.addMethod("requiredif", function (value, element, params) {
        if ($(element).val() != '') return true;
        var other = $('#' + params.other);
        var actualVal = $(other).attr('value').toLowerCase();
        var targetVal = params.value.toLowerCase();
        if (targetVal === actualVal) {
            return $.validator.methods.required.call(this, value, element, params);
        }
        return true;
    });
    $.validator.unobtrusive.adapters.add("requiredif", ["other", "comp", "value"],
        function (options) {
            options.rules['requiredif'] = {
                other: options.params.other,
                comp: options.params.comp,
                value: options.params.value
            };
            options.messages['requiredif'] = options.message;
        }
    );
    //validation for RegularExpressionIf
    $.validator.addMethod("regularexpressionif", function (value, element, params) {
        var re = new RegExp(params.regex);
        if ($(element).val().match(re)) { return true; }
        var other = $('#' + params.other);
        var actualVal = $(other).attr('value').toLowerCase();
        var targetVal = params.value.toLowerCase();
        if (targetVal === actualVal) {
            return $.validator.methods.regex.call(this, value, element, params);
        }
        return true;
    });
    $.validator.unobtrusive.adapters.add("regularexpressionif", ["regex", "other", "comp", "value"],
        function (options) {
            options.rules['regularexpressionif'] = {
                regex: options.params.regex,
                other: options.params.other,
                comp: options.params.comp,
                value: options.params.value
            };
            options.messages['regularexpressionif'] = options.message;
        }
    );
} (jQuery));