const request = require('request');
const url = "http://img-visor-jn.nunez-technologies.com/maintenance.php";

request({
    url: url,
    headers: {
        "Authorization": "Basic aW1nLXZpc29yLWpuLTQ4OjJJSUQsMCRnYTJdSA=="
    }
},
        function (error, response, body) {
            console.error('error:', error); // Print the error if one occurred
            console.log('statusCode:', response && response.statusCode); // Print the response status code if a response was received
            console.log('body:', body); // Print the HTML for the Google homepage.
        }
);