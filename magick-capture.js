function Process() {
    const process = require('child_process');   
    const moment = require('moment');
	let momentString = moment().format("YYYYMMDDHHmmss");
	let fileName = "sc_"+momentString+".jpg";
	
	console.log("FILENAME: ",fileName);
	
	var ls = process.spawn('cmd.exe', ['/c','magick-capture-exec.bat',fileName]);
	 
	
    ls.stdout.on('data', function (data) {
      console.log("STDOUT: ",data);
    });
    ls.stderr.on('data', function (data) {
      console.log("STDERR: ",data);
    });
	
    ls.on('close', function (code) {
       if (code == 0)
            console.log('Stop');
       else
            console.log('Start');
    });
};

Process();