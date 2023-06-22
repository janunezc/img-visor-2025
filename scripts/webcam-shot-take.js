//Available in nodejs
const NodeWebcam = require("node-webcam");
const process = require('child_process');
const moment = require('moment');

//Default options
const opts = {
    width: 1280, //No effect over commandCam.exe
    height: 720, //No effect over commandCam.exe
    quality: 50, //No effect over commandCam.exe
    frames: 60,
    delay: 0,
    saveShots: true,
    output: "jpeg",
    device: false,
    callbackReturn: "location",
    verbose: true
};

const Webcam = NodeWebcam.create(opts);

let momentString = moment().format("YYYYMMDDHHmmss");
let tenthValidator = moment().format("mm");


let fileName = "images_temp\\sc_" + momentString;

Webcam.capture(fileName, function (err, data) {
    console.log("Picture taken", data);

    fileName += ".jpg";
    var ls = process.spawn('cmd.exe', ['/c', 'webcam-shot-reduce.bat', fileName]);
});


