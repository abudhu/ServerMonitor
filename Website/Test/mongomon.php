<?php 
try {
	// connect to MongoDB
	//$m = new Mongo("localhost:27017");
	$m = new Mongo("mongodb://bptest:2kS9oxQs@staff.mongohq.com:10063/bptest_db");
} catch (Exception $e) {
	echo 'Caught exception: ',  $e->getMessage(), "\n";
}

// select a database
$db = $m->bptest_db;

// search parameters
$cursor = $db->datacollection->find(array("computer_Name"=>"AMITBUDHUA681"))->sort(array("unix_timestamp"=>-1))->limit(1);
// do the search
$result = $cursor->getNext();

echo $result['computer_Name'].",".floor($result['total_weight']/4);


// $cursor = $db->datacollection->find(array("computer_Name"=>"DAUCA-JJACOBSON"))->sort(array("unix_timestamp"=>-1))->limit(1);
// do the search
// $result = $cursor->getNext();

echo ",".$result['computer_Name'].",".floor($result['total_weight']/4);

?>
