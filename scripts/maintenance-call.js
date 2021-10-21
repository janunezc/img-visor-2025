const request = require('request');
const url = "https://costaricamakers.com/img-visor-jn/maintenance.php";

request({
    url: url,
    headers: {
        "Authorization": "Basic aW1nLXZpc29yLWpuOiFBN1hzQWYuTzBVUg=="
    }
},
        function (error, response, body) {
            console.error('error:', error); // Print the error if one occurred
            console.log('statusCode:', response && response.statusCode); // Print the response status code if a response was received
            console.log('body:', body); // Print the HTML for the Google homepage.
        }
);