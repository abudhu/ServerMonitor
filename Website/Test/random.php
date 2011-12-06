<?php
	$vals = 5;

	$rmin = 0;
	$rmax = 100;

	echo rand($rmin, $rmax);
	for ($i=1; $i<$vals; $i++) echo ','.rand($rmin, $rmax);
?>
