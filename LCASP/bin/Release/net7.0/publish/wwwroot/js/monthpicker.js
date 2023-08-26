$(".input-datepicker").daterangepicker({
    minDate: moment().subtract(50, 'years'),
    maxDate: moment(),
    callback: function (startDate, endDate, period) {
        $(this).val(startDate.format('L') + '-' + endDate.format('L'));
    }
});