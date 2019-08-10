<?php
$version = "0.1";

$apiCommand = filter_input(INPUT_GET, "apiCommand");
if ($apiCommand == "getnames") {
    $files = array();
    $images = array();
    $folder = ".";

    if ($handle = opendir($folder)) {
        while (false !== ($file = readdir($handle))) {
            if ($file != "." && $file != "..") {
                $files[filemtime($file)] = $file;
            }
        }

        closedir($handle);

        ksort($files);

        foreach ($files as $file) {
            $extension = substr($file, -3);
            if ($extension == "png" || $extension == "jpg") {
                $images[] = $folder . "/" . $file;
            }
        }
    }
    header('Content-Type: application/json');
    echo json_encode($images);
    die();
}
?>
<!DOCTYPE html>
<html>
    <head>
        <title>img-visor rev. <?php echo $version; ?></title>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <script src="https://code.jquery.com/jquery-3.4.1.min.js" type="text/javascript"></script>

    </head>
    <body style="text-align:center">
        <h1><pre>img-visor <?php echo $version; ?></pre></h1>
        <button id="btnREW"> << </button> <button id="btnPause"> || </button> <button id="btnFF"> >> </button>
        <p id="theContainer">Not Loaded</p>
        <img id="theImage" style="min-width: 400px; max-width:800px; width:100%; margin:0 auto; display:block" src="screenshot 08-08-2019-204928.png" alt="IMAGE LOADING..." />
        <script type="text/javascript">

            $(document).ready(function () {
                var filesArray = [];
                var timer;
                var jqxhr = $.ajax("index.php?apiCommand=getnames")
                        .done(function (data) {
                            filesArray = (data);
                            timerFunction();
                        })
                        .fail(function (err) {
                            console.log("error", err);
                        })
                        .always(function () {});
                var index = 0;
                var direction = "FF";
                var paused = false;

                $("#theImage").on("load", setTimer);
                $("#btnPause").click(function () {
                    paused = !paused;
                    if (paused) {
                        $("#btnPause").text(" > ");
                    } else {
                        $("#btnPause").text(" || ");
                    }
                    setTimer();
                });

                $("#btnREW").click(goBack);
                $("#btnFF").click(advance);

                function setTimer() {
                    clearTimeout(timer);
                    timer = setTimeout(timerFunction, 300);
                }

                function advance() {
                    if (index < filesArray.length - 1) {
                        index++;
                    } else {
                        index = 0;
                    }
                    $("#theImage").attr("src", "" + filesArray[index]);
                }

                function goBack() {
                    if (index > 0) {
                        index--;
                    } else {
                        index = filesArray.length - 1;
                    }
                    $("#theImage").attr("src", "" + filesArray[index]);
                }

                function timerFunction() {
                    $("#theContainer").html("IDX:" + (index + 1)     + " OF " + filesArray.length + " FILE:" + filesArray[index]);

                    if (!paused) {
                        advance();
                    }
                }
            });


        </script>
    </body>
</html>
