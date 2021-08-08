max select all;
max delete;

arr = #()
spheres = #()
k=1;
in_name = "D:\\Startpoint.txt";
in_file = openFile in_name
Rad = readValue in_file
a=sphere pos: [0,0,0] radius: Rad segments:40;	

--assigns a controller:
a.visibility = bezier_float()
--set the current value to semi-visible:
a.visibility.controller.value = 0.5
--animate the value to 0 on frame 100
animate on at time 100 a.visibility.controller.value = 0.0

if in_file != undefined then
(
	while not(eof in_file )
	do
		(
			x = readValue in_file;
			y = readValue in_file;
			z = readValue in_file;
			arr=[x,y,z]; 
			
			r = readValue in_file;
			
		spheres[k]=sphere() ;
			spheres[k].pos=arr;
			spheres[k].radius=r;
			spheres[k].wireColor=random [0,0,0][255,255,255];
			k+=1;
		)	
		close in_file;
 )
 print spheres
 