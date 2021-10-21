<b>HELLO! Maintenance Routine goes here.</b>
<?php

$days = 7;  
$path = './images/';  
$filetypes_to_delete = array("jpg");  
  
// Open the directory  
if ($handle = opendir($path))  
{  
    // Loop through the directory  
    while (false !== ($file = readdir($handle)))  
    {  
        // Check the file we're doing is actually a file 
		echo("FILE $file <br/>");
        if (is_file($path.$file))  
        {  
            $file_info = pathinfo($path.$file);  
            if (isset($file_info['extension']) && in_array(strtolower($file_info['extension']), $filetypes_to_delete))  
            {  
                // Check if the file is older than X days old  
                if (filemtime($path.$file) < ( time() - ( $days * 24 * 60 * 60 ) ) )  
                {  
                    // Do the deletion  
                    unlink($path.$file);  
					echo("DELETED!");
                }  
            }  
        }  
    }  
} 