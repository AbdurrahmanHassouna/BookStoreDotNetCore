// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    // 1. Configure global $.ajaxSetup: attach RequestVerificationToken header for non-GET requests
    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            if (!/^(GET|HEAD|OPTIONS|TRACE)$/i.test(settings.type)) {
                var token = $('input[name="__RequestVerificationToken"]').val();
                if (token) {
                    xhr.setRequestHeader("RequestVerificationToken", token);
                }
            }
        }
    });

    // Helper: update #cartItemCount from cart elements or fallback
    function updateCartItemCount(count) {
        if (typeof count !== 'undefined' && count !== null) {
            $('#cartItemCount').html(count);
            return;
        }

        var total = 0;
        var hasItems = false;
        $('#Cart .quantity-select').each(function () {
            hasItems = true;
            var val = parseInt($(this).val(), 10);
            if (!isNaN(val)) {
                total += val;
            }
        });

        if (hasItems) {
            $('#cartItemCount').html(total);
        } else if ($('#Cart').length > 0) {
            // Cart is present but no quantity-select elements found (e.g. empty cart)
            $('#cartItemCount').html(0);
        }
    }

    // 2. Delegated handler for Add to Cart button
    $(document).on('click', '.addToCartBtn', function (e) {
        e.preventDefault();
        var $btn = $(this);
        var bookId = $btn.data('book-id') || $btn.siblings('input[name="Id"]').val();
        if (!bookId) return;

        var originalHtml = $btn.html();
        $btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Adding...');

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { bookId: bookId },
            success: function (result) {
                $('#cartItemCount').html(result);
                if (typeof toastr !== 'undefined') {
                    toastr.success('Item added to cart!');
                }
                $btn.prop('disabled', false).html(originalHtml);
            },
            error: function () {
                if (typeof toastr !== 'undefined') {
                    toastr.error('An error occurred while adding the item to the cart.');
                }
                $btn.prop('disabled', false).html(originalHtml);
            }
        });
    });

    // 3. Delegated handler for Quantity Select change
    $(document).on('change', '.quantity-select', function () {
        var $select = $(this);
        var bookId = $select.data('book-id');
        var quantity = $select.val();

        $('#Cart').css('opacity', '0.5');

        $.ajax({
            url: '/Cart/UpdateCart',
            type: 'POST',
            data: { bookId: bookId, quantity: quantity },
            success: function (result) {
                $('#Cart').html(result);
                $('#Cart').css('opacity', '1');
                updateCartItemCount();
            },
            error: function () {
                if (typeof toastr !== 'undefined') {
                    toastr.error('An error occurred while updating the cart.');
                }
                $('#Cart').css('opacity', '1');
            }
        });
    });

    // 4. Delegated handler for Delete Cart Item button
    $(document).on('click', '.delete-cart-item', function (e) {
        e.preventDefault();
        var $btn = $(this);
        var bookId = $btn.data('book-id');

        $('#Cart').css('opacity', '0.5');

        $.ajax({
            url: '/Cart/DeleteCartItem',
            type: 'POST',
            data: { id: bookId },
            success: function (result) {
                $('#Cart').html(result);
                $('#Cart').css('opacity', '1');
                updateCartItemCount();
            },
            error: function () {
                if (typeof toastr !== 'undefined') {
                    toastr.error('An error occurred while removing the item from the cart.');
                }
                $('#Cart').css('opacity', '1');
            }
        });
    });
});
