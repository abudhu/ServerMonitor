<?php

try 
{
	// connect to MongoDB
	$m = new Mongo("mongodb://bptest:2kS9oxQs@staff.mongohq.com:10063/bptest_db");
} 
catch (Exception $e) 
{
	echo 'Caught exception: ',  $e->getMessage(), "\n";
}

// Select the database
$db = $m->bptest_db;

// Search parameters based on our input
//this needs to be put into an array loop, so that you dont have to get all the weights over and over and dupe code.
$cursor = $db->datacollection->find(array("computer_Name"=>"AMITBUDHUA681"))->sort(array("unix_timestamp"=>-1))->limit(1);
// Get the result.
$result = $cursor->getNext();


//Get all of our weights
$driveWeight = $result['driveWeight'];
$iopWeight = $result['iopWeight'];
$cpuWeight = $result['cpuWeight'];
$memWeight = $result['memWeight'];
$netWeight = $result['netWeight'];

// Get total weight (dividing by 2560 (512 * 5 items) to get % health out of 100) then making it a whole number
$total = floor(((($driveWeight + $iopWeight + $cpuWeight + $memWeight + $netWeight) / 2560) * 100));

// Return the value to the page along with the computer name for Display
echo $result['computer_Name'].",".$total;

?>
