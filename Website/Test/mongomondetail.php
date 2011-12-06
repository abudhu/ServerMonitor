<?php
try {
	// connect to MongoDB
	//$m = new Mongo("localhost:27017");
	$m = new Mongo("mongodb://bptest:2kS9oxQs@staff.mongohq.com:10063/bptest_db");
} catch (Exception $e) {
	echo 'Caught exception: ',  $e->getMessage(), "\n";
}

switch ($_GET['host']) {
	case 1:
		$host = 'ABUDHU-PC';
		break;
	case 2:
		$host = 'DAUCA-JJACOBSON';
		break;
	default:
		die ('no host name specified');
}


// select a database
$db = $m->bptest_db;

// search parameters
$cursor = $db->datacollection->find(array("computer_Name"=>"$host"))->sort(array("unix_timestamp"=>-1))->limit(1);
// do the search
$result = $cursor->getNext();

if ($_GET['debug']==1) {
	echo "<pre>";
	print_r($result);
	echo "</pre>";
} // if

$cpu = floor($result['cpu_weight']/512*100);
	if ($cpu>100) $cpu = 100;
$mem = floor($result['mem_weight']/512*100);
	if ($mem>100) $mem = 100;
$disk = floor($result['disk_info'][0]*100);
$iop = floor($result['disk_read']+$result['disk_write']);
	if ($iop>100) $iop = 100;
$net = 50;

//echo $result['computer_Name'].",".floor($result['total_weight']/4);
echo "$cpu,$mem,$disk,$iop,$net";

?>
