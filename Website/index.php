<!DOCTYPE HTML>
<html>
<head>
	<style>
		body { padding:0px; margin:0px; font-size:12px; font-family:sans-serif; }
		#myCanvas { position:absolute; width:100%; height:100%; overflow:hidden; }
	</style>
	<script>

	var servernames = new Array();
	var servercount = 0;
	var originalvals = new Array();
	var targetvals = new Array();
	var currentvals = new Array();
	var refreshinterval = 30000;
	var stepsnum = 100;
	var currentstep = 0;

	var debug = false;

/***********************************************
* Dynamic Ajax Content- © Dynamic Drive DHTML code library (www.dynamicdrive.com)
* This notice MUST stay intact for legal use
* Visit Dynamic Drive at http://www.dynamicdrive.com/ for full source code
***********************************************/
function ajaxpage(url, cid){
	containerid = "rdiv"+cid;
	var page_request = new XMLHttpRequest()
	page_request.onreadystatechange=function(){ loadpage(page_request, cid) }
	var bustcacheparameter=(url.indexOf("?")!=-1)? "&"+new Date().getTime() : "?"+new Date().getTime()
	page_request.open('GET', url+bustcacheparameter, true)
	page_request.send(null)
}

function loadpage(page_request, cid){
	if (page_request.readyState == 4 && (page_request.status==200 || window.location.href.indexOf("http")==-1))
	document.getElementById("rdiv"+cid).innerHTML=page_request.responseText
	else return;

	// something to animate from
	for (i=0; i<targetvals.length; i++)  originalvals[i] = targetvals[i];  

	splitstring = new String(document.getElementById("rdiv"+cid).innerHTML);
	// something to animate to
	var tempvals = splitstring.split(',');
	// tempval[1] = the number returned from mongomon

	// the number of servers we represent in this tool can change dynamically
	//because the array consist of two numbers (name + value) he is dividing by 2 to remove the "number" and get the server amount
	servercount = tempvals.length / 2;

	// set up the arrays that contain the server names and health ratings
	for (i=0; i<servercount; i++) {
		servernames[i] = tempvals[(i*2)]; // 0 * 2 = [0] which is array position of name
		targetvals[i] = parseInt(tempvals[((i*2)+1)]); // 0 * 2 = [0] + 1 = [1] which is value of health
	} // for

	currentstep = 0;

} // setValues

function drawValues(){
	//currentvals = originalvals;
	//floor rounds the number DOWN (removing the decimal)
	for (i=0; i<originalvals.length; i++) {
		currentvals[i] = Math.floor(originalvals[i] + ((targetvals[i]-originalvals[i])/stepsnum*currentstep));
	} // for
	//drawme("myCanvas"+cid, currentvals);
	currentstep++;

	if (currentstep >= stepsnum) {
		ajaxpage("mongomon.php", 0);
		currentstep = 0;
	} // if

	
		var canvas = document.getElementById('myCanvas');
		var context = canvas.getContext("2d");

		var w = canvas.width;
		var h = canvas.height;

//	context.clearRect(0,0,w,h);
	context.fillStyle = "rgb(0,0,0)"; 
	context.fillRect(0,0,w,h);

		for (i=0; i<servercount; i++) {
			context.font = '18px sans-serif';
			context.textBaseline = 'top';

			var textmetric = context.measureText(servernames[i]+"  "+(Math.floor(currentvals[i]/255*100))+"%"); // this is where I ACTUALLy want to remove the 255 but removed from below method

			//var g = 255-currentvals[i]; // he is setting the GREEN color here for RGB
			
			var r = 255 - ((Math.floor(currentvals[i])) * 2);
			var g = 255- ((100 - (Math.floor(currentvals[i]))) * 2);
			
			//context.fillStyle = "rgb("+currentvals[i]+",0,"+g+")";
			context.fillStyle = "rgb("+r+","+g+",0)"; // this sets teh color of the box.
			context.fillRect(40, (20+(60*i)), 260, 50 );
			//context.fillRect(40, (20+(50*i)), textmetric.width+20, 40 );

			context.fillStyle = "rgb(255,255,255)"; 
			context.fillText(servernames[i]+"  "+(Math.floor(currentvals[i]))+"%", 50, 22+(60*i+15));
		} // for
} // drawValues

function init() {
	adjustViewport();
	ajaxpage('mongomon.php', 0); // there are two because when it goes to loadpage function 1 array holds all the names
	ajaxpage('mongomon.php', 0); // the other array holds all the values.  And they are rebuilt in draw values
	setInterval("drawValues()", refreshinterval/stepsnum);
} // init()

function adjustViewport() {
	document.getElementById('myCanvas').width = window.innerWidth;
	document.getElementById('myCanvas').height = window.innerHeight;

	if (debug) document.getElementById('debug').innerHTML = window.innerWidth+" x "+window.innerHeight;
} // adjustViewport


window.onload = window.onresize = function() {
	adjustViewport();
} // window.onload and window.onresize

	</script>
</head>
<body onload='init()'>
<canvas id='myCanvas'></canvas>

<div id='rdiv0' style="position:absolute; margin-left:200px; color:white; display:none"></div>
</body>
</html>
