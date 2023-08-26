
$('.fa-star').mouseover(function () {
    $(this).prevAll().addBack().addClass('checked');
    $(this).nextAll().removeClass('checked');
});


$('.fa-star').click(function () {
    alert($(this).attr('value'));
});