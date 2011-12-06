<?php
	$vals = 8;

	$rmin = 0;
	$rmax = 255;

	echo 'server0,'.rand($rmin, $rmax);
	for ($i=1; $i<$vals; $i++) echo ",server$i,".rand($rmin, $rmax);
?>
