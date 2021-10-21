<?php
/*
ESTE ES UN ARCHIVO DE USO DIDÁCTICO QUE SE DISTRIBUYE DE FORMA LIBRE BAJO LA LICENCIA MIT.
VER: https://en.wikipedia.org/wiki/MIT_License
ESTE PROGRAMA ES BRINDADO DE FORMA GRATUITA Y LIBRE CON LA MEJOR INTENCIÓN DE FACILITAR SU APRENDIZAJE Y USO.
NO ACEPTAMOS NINGUN TIPO DE RESPONSABILIDAD POR EL USO QUE SE LE DE A ESTE PROGRAMA.
*/
$version = "1.101";

$apiCommand = filter_input(INPUT_GET, "apiCommand");
if ($apiCommand == "getnames") {
    $files = array();
    $images = array();
    $folder = "./images";

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
            .image-thumbnail{
                width:24%;
                display:inline-block;
                border-color: gray;
                border-style: solid;
                border-width: 1px;
                margin:5px;
            }
        </style>
    </head>
    <body style="text-align:center">
        <h1><pre>img-visor Timeline<?php echo $version; ?></pre></h1>
        <div id="divImages">
        </div>
        <button id="cmdLoadMore" class="btn btn-primary">Load More...</button>

        <script src="https://kit.fontawesome.com/db2d8f91f5.js"></script>
        <script src="https://code.jquery.com/jquery-3.4.1.min.js" type="text/javascript"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>

        <script type="text/javascript">
            $(document).ready(function () {
                let imageNames = [];

                setupEvents();
                retrieveImageNames();

                /***
                 * Loads available image names into imageNames variable.
                 * @returns {undefined}
                 */
                function retrieveImageNames() {
                    $("#cmdLoadMore").html("loading...");

                    $.ajax("index.php?apiCommand=getnames")
                            .done(function (data) {
                                imageNames = (data);
                                loadImages(150);
                                $("#status").html("Images found: " + data.length);
                            })
                            .fail(function (err) {
                                console.log("error", err);
                            })
                            .always(function () {});
                }

                var nextIndex = 0;
                var lastIndex = 0;

                function formatDateTime(dtText) {
                    let year = dtText.substring(0, 4);
                    let month = dtText.substring(4, 6);
                    let day = dtText.substring(6, 8);
                    let hour = dtText.substring(8, 10);
                    let minute = dtText.substring(10, 12);
                    return year + "-" + month + "-" + day + " " + hour + ":" + minute;
                }
                
                var prevDate = undefined;
                function loadImages(qty) {
                    $("#cmdLoadMore").html("loading...");
                    if (nextIndex === 0)
                        nextIndex = imageNames.length - 1;
                    console.log("-------------------", "RETREIVING IMAGES", qty, nextIndex);
                    
                    for (let idx = nextIndex; (idx >= nextIndex - qty) && idx >= 0; idx--) {
                        console.log(imageNames[idx], idx);
                        let imgPath = "./" + imageNames[idx];
                        let dateTimeText = imgPath.substring(imgPath.indexOf("sc_") + 3, imgPath.indexOf("sc_") + 15);
                        let q = idx;
                        dateTimeText = formatDateTime(dateTimeText) + " (" + (q) + ")";
                        let curDate = dateTimeText.substring(0,10);
                        let imgHTMLCode = "";
                        
                        if(prevDate!==curDate){
                            prevDate = curDate;
                            imgHTMLCode = "<hr /> <h1>" + curDate + "</h1>";
                            
                        }
                        imgHTMLCode += "<div id='div-tn-" + idx + "' class='image-thumbnail'>" +
                                "<a target='_blank' href='" + imgPath + "'><img id='img-" + idx + "' style='width:100%;' src='" + imgPath + "'></img></a>&nbsp;" +
                                dateTimeText;
                        $("#divImages").html($("#divImages").html() + imgHTMLCode);
                        lastIndex = idx;
                    }

                    nextIndex = lastIndex - 1;
                    $("#cmdLoadMore").html("Load More");
                }

                function setupEvents() {
                    $("#cmdLoadMore").click(function () {
                        $("#cmdLoadMore").html("loading...");
                        loadImages(150);
                    });
                }

            });
        </script>
    </body>
</html>
