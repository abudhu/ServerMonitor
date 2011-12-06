<?php
	$shapes = 2;
	$w = $h = 100;

?><!DOCTYPE HTML>
<html>
<head>
	<script>

	var originalvals = new Array();
	var targetvals = new Array();
	var currentvals = new Array();
	var refreshinterval = 10000;
	var stepsnum = 200;
	var currentstep = new Array();


	//window.onload = function(){
	function drawme(ccc, lengths) {
		//document.getElementById("debug").innerHTML += ccc;
		var canvas = document.getElementById(ccc);
		var context = canvas.getContext("2d");

		var theta = 0;

		var w = <?=$w?>;
		var h = <?=$h?>;
		var ll = w/2;

		context.clearRect(0,0,w,h);

		context.beginPath();
		context.moveTo(ll, (h/2)-ll);
		for (l=1; l<lengths.length; l++) {
			theta = (2*Math.PI) * ( l / lengths.length );
   			context.lineTo( ll+(ll * Math.sin(theta)), ll-(ll * Math.cos(theta)) );
		} // for
		context.lineTo(ll, (h/2)-ll);
		context.lineWidth = 1;
		context.strokeStyle = "#000000";
		context.stroke();
		context.fillStyle = "#eeeeee";
		context.fill();

		context.beginPath();
		context.moveTo(ll, (h/2)-(lengths[0]/100*ll));
		for (l=1; l<lengths.length; l++) {
			theta = (2*Math.PI) * ( l / lengths.length );
   			//context.lineTo( ll+(ll * Math.sin(theta)), ll-(ll * Math.cos(theta)) );

   			context.lineTo( ll+((lengths[l]/100 * ll)* Math.sin(theta)), ll-((lengths[l]/100*ll) * Math.cos(theta)) );
		} // for
		context.lineTo(ll, (h/2)-(lengths[0]/100*ll));
		context.lineWidth = 1;
		context.fillStyle = "#aaaacc";
		context.fill();


		context.beginPath();
		context.moveTo(ll, ll);
		for (l=0; l<lengths.length; l++) {
			theta = (2*Math.PI) * ( l / lengths.length );
   			context.lineTo( ll+(((lengths[l]/100*ll)-2) * Math.sin(theta)), ll-(((lengths[l]/100*ll)-2) * Math.cos(theta)) );
			context.moveTo(ll, ll);
		} // for
		context.lineWidth = 2;
		context.lineCap = "round";
		context.strokeStyle = "#bbbbbb";
		context.stroke();
	};


/***********************************************
* Dynamic Ajax Content- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
* This notice MUST stay intact for legal use
* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
***********************************************/

var bustcachevar=1 //bust potential caching of external pages after initial request? (1=yes, 0=no)
var loadedobjects=""
var rootdomain="http://"+window.location.hostname
var bustcacheparameter=""

function ajaxpage(url, cid){
	containerid = "rdiv"+cid;
	//document.getElementById("debug").innerHTML += "ajaxpage: "+containerid+"<br>";
	var page_request = false
	if (window.XMLHttpRequest) { // if Mozilla, Safari etc
		page_request = new XMLHttpRequest()
	} else if (window.ActiveXObject){ // if IE
		try {
			page_request = new ActiveXObject("Msxml2.XMLHTTP")
		} catch (e){
			try{
				page_request = new ActiveXObject("Microsoft.XMLHTTP")
			} catch (e){}
		}
	} else {
		return false
	}

	page_request.onreadystatechange=function(){ loadpage(page_request, cid) }

	if (bustcachevar) {//if bust caching of external page
		bustcacheparameter=(url.indexOf("?")!=-1)? "&"+new Date().getTime() : "?"+new Date().getTime()
	} // if
	page_request.open('GET', url+bustcacheparameter, true)
	page_request.send(null)
};

function loadpage(page_request, cid){
	if (page_request.readyState == 4 && (page_request.status==200 || window.location.href.indexOf("http")==-1))
	document.getElementById("rdiv"+cid).innerHTML=page_request.responseText
	setValues(cid);
};

function setValues(cid) {
	originalvals[cid] = targetvals[cid];
	splitstring = new String(document.getElementById("rdiv"+cid).innerHTML);
	targetvals[cid] = splitstring.split(',');

	for (i=0; i<targetvals[cid].length; i++) {
		targetvals[cid][i] = parseFloat(targetvals[cid][i]);
	} // for
	currentstep[cid] = 0;

} // setValues

function drawValues(cid){
	//currentvals = originalvals;
	for (i=0; i<originalvals[cid].length; i++) {
		currentvals[cid][i] = originalvals[cid][i] + ((targetvals[cid][i]-originalvals[cid][i])/stepsnum*currentstep[cid]);
	} // for
	drawme("myCanvas"+cid, currentvals[cid]);
	currentstep[cid]++;

	if (currentstep[cid] >= stepsnum) ajaxpage("mongomondetail.php?host="+(cid+1), cid);

	document.getElementById('debug').innerHTML = "";
	document.getElementById('debug').innerHTML += currentstep+"/"+stepsnum+"<br>";
	
} // drawValues


<?
	$timer = 0;
	for ($i=0; $i<$shapes; $i++) {
		$host = $i+1;
		$timer += 100;
		$ajax .= "setTimeout('ajaxpage(\"mongomondetail.php?host=$host\", $i)', $timer);\n";
		$timer += 100;
		$ajax .= "setTimeout('ajaxpage(\"mongomondetail.php?host=$host\", $i)', $timer);\n";

		$cvals .= "currentvals[$i] = new Array();\n";

		$timeouts .= "setTimeout('setInterval(\"drawValues($i)\", refreshinterval/stepsnum)', ".(($shapes * 2 * 100) + 200).");\n";

		$canvases .= "<canvas id='myCanvas$i' width='$w' height='$h'></canvas>\n";
		//$rdivs .= "<div id='rdiv$i' style='display:none;'>rdiv$i</div>\n";
		$rdivs .= "<div id='rdiv$i'>rdiv$i</div>\n";
	} // for
?>

function init() {
	<?=$ajax?>

	<?=$cvals?>

	<?=$timeouts?>
} // init()

	</script>
</head>
<body onload='init()'>
<?=$canvases?>
<br>
<div id='debug'>this is debug info</div>
<br>
<?=$rdivs?>
</body>
</html>
