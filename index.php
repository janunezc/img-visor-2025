<?php
$version = "1.1";

$apiCommand = filter_input(INPUT_GET, "apiCommand");
if ($apiCommand == "getnames") {
    $files = array();
    $images = array();
    $folder = ".";

    if ($handle = opendir($folder)) {
        while (false !== ($file = readdir($handle))) {
            if ($file != "." && $file != "..") {
                //$files[filemtime($file)] = $file;
                $files [$file]= $file;
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
        <style>
            .slidecontainer {
                width: 100%;
            }

            .slider {
                -webkit-appearance: none;
                width: 100%;
                height: 25px;
                background: #d3d3d3;
                outline: none;
                opacity: 0.7;
                -webkit-transition: .2s;
                transition: opacity .2s;
            }

            .slider:hover {
                opacity: 1;
            }

            .slider::-webkit-slider-thumb {
                -webkit-appearance: none;
                appearance: none;
                width: 25px;
                height: 25px;
                background: #4CAF50;
                cursor: pointer;
            }

            .slider::-moz-range-thumb {
                width: 25px;
                height: 25px;
                background: #4CAF50;
                cursor: pointer;
            }
        </style>
    </head>
    <body style="text-align:center">
        <h1><pre>img-visor <?php echo $version; ?></pre></h1>
        <button id="btnFastRewind" class="btn btn-primary"><i class="fas fa-fast-backward"></i></button>
        <button id="btnBack" class="btn btn-primary"><i class="fas fa-backward"></i></button>
        <button id="btnPause" class="btn btn-primary"><i class="fas fa-play"></i></button>
        <button id="btnForward" class="btn btn-primary"><i class="fas fa-forward"></i></button>
        <button id="btnFastForward" class="btn btn-primary"><i class="fas fa-fast-forward"></i></button>
        <pre id="imgData"> - </pre>

        <div class="slidecontainer">
            <input id="sliderX" type="range" min="1" max="1" value="1" style="width:400px" disabled>    
            <!--<input type="range" min="1" max="100" value="50" class="slider" id="myRange">-->
        </div>

        <img id="theImage" style="min-width: 400px; max-width:800px; width:100%; margin:0 auto; display:none;" src="" alt="..." />

        <script src="https://kit.fontawesome.com/db2d8f91f5.js"></script>
        <script src="https://code.jquery.com/jquery-3.4.1.min.js" type="text/javascript"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>

        <script type="text/javascript">
            $(document).ready(function () {
                var filesArray = [];
                var timer;
                setupEvents();


                var index = 0;
                var paused = true;

                loadImagesList();

                function loadImagesList() {
                    $("#imgData").html("Loading images list...");
                    var jqxhr = $.ajax("index.php?apiCommand=getnames")
                            .done(function (data) {
                                //filesArray = (data.sort());
                                filesArray = (data);
                                $("#sliderX").attr("max", filesArray.length);
                                updateImage();
                                timerFunction();
                            })
                            .fail(function (err) {
                                console.log("error", err);
                            })
                            .always(function () {});

                }
                function setupEvents() {
                    $('input[type=range]').on('input', function () {
                        $(this).trigger('change');
                    });

                    $("#sliderX").change(respondToSliderChange);

                    $("#btnPause").click(function () {
                        paused = !paused;
                        if (paused) {
                            $("#btnPause i").removeClass("fa-pause").addClass("fa-play");
                        } else {
                            $("#btnPause i").removeClass("fa-play").addClass("fa-pause");
                        }
                        setTimer();
                    });

                    $("#btnBack").click(goBack);
                    $("#btnForward").click(advance);

                    $("#btnFastRewind").click(goBackTen);
                    $("#btnFastForward").click(advanceTen);


                    $("#theImage").on("load", setTimer);

                }
                function respondToSliderChange() {

                    console.log("CHANGE DETECTED",
                            $("#sliderX").value,
                            $(this).val(),
                            filesArray[$(this).val()]
                            );
                    index = 1 * $(this).val() - 1;
                    updateImage();
                }

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

                    $("#sliderX").val(index);
                    updateImage();
                }

                function advanceTen() {
                    if (index < filesArray.length - 10) {
                        index += 10;
                    } else {
                        index = filesArray.length - 1;
                    }

                    $("#sliderX").val(index);
                    updateImage();
                }

                function goBackTen() {
                    if (index > 10) {
                        index = index - 10;
                    } else {
                        index = 0
                    }

                    $("#sliderX").val(index);
                    updateImage();
                }

                function goBack() {
                    if (index > 0) {
                        index--;
                    } else {
                        index = filesArray.length - 1;
                    }

                    $("#sliderX").val(index);
                    updateImage();
                }

                var timingLimiter;
                var previousIndex = -1;
                function updateImage() {
                    var pictureNumber = index + 1;
                    console.log(pictureNumber);
                    $("#imgData").html("IMG#: " + pictureNumber + " OF " + filesArray.length + "<br />" + filesArray[index]);
                    $("#theImage").show();
                    $("#sliderX").removeAttr("disabled");

                    if (previousIndex !== index) {
                        previousIndex = index;
                        if (timingLimiter) {
                            clearTimeout(timingLimiter);
                        }
                        timingLimiter = setTimeout(function () {
                            $("#theImage").attr("src", "" + filesArray[index]);
                        }, 400);
                    }

                }

                function timerFunction() {
                    updateImage();
                    if (!paused) {
                        advance();
                    }
                }
            });
        </script>
    </body>
</html>
