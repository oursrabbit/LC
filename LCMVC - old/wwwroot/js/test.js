

$.ajax({
    url: 'http://localhost:5000/WeatherForecast', type: 'GET', success: function (data) {
        // Handle the response data here 
        alert(JSON.stringify(data));
    }, error: function (error) {
        // Handle any errors that occur during the request
        console.log(error);
    }
});