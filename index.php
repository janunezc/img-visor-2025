<?php
$version = "0.2";

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
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous">

    </head>
    <body style="text-align:center">
        <h1><pre>img-visor <?php echo $version; ?></pre></h1>
        <button id="btnREW" class="btn btn-primary"><i class="fas fa-backward"></i></button>
        <button id="btnPause" class="btn btn-primary"><i class="fas fa-play"></i></button>
        <button id="btnFF" class="btn btn-primary"><i class="fas fa-forward"></i></button>
        <pre id="imgData"> - </pre>
        <img id="theImage" style="min-width: 400px; max-width:800px; width:100%; margin:0 auto; display:block" src="" alt="..." />

        <script src="https://kit.fontawesome.com/db2d8f91f5.js"></script>
        <script src="https://code.jquery.com/jquery-3.4.1.min.js" type="text/javascript"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>

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
                var paused = true;

                $("#theImage").on("load", setTimer);
                $("#btnPause").click(function () {
                    paused = !paused;
                    if (paused) {
                        $("#btnPause i").removeClass("fa-pause").addClass("fa-play");
                    } else {
                        $("#btnPause i").removeClass("fa-play").addClass("fa-pause");
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
                    $("#imgData").html("IDX:" + (index + 1) + " OF " + filesArray.length + "<br />" + filesArray[index]);

                    if (!paused) {
                        advance();
                    }
                }
            });
        </script>
    </body>
</html>
