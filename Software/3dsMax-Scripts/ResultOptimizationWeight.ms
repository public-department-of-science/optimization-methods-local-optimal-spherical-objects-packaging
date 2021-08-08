-- select all objects and delete selected elements
max select all;
max delete;
-- initial arrays
spheres = #()
k=1; -- count
-- chooise directories; open txt file
in_name = "D:\\Coordinate.txt"; 

in_file = openFile in_name;

Rad = readValue in_file;
a=sphere pos: [0,0,0] radius: Rad segments:60;
a.name="MainSph";

--assigns a controller:
a.visibility = bezier_float()
--set the current value to semi-visible:
a.visibility.controller.value = 0.5
--animate the value to 0 on frame 100
animate on at time 100 a.visibility.controller.value = 0.0

-- reading while EOF
if in_file != undefined then
(
	while not(eof in_file)
	do
		(
			x = readValue in_file;
			y = readValue in_file;
			z = readValue in_file;
			temp=[x,y,z]; 
			r = readValue in_file;
			weight = readValue in_file;
			
			spheres[k]=sphere();
			spheres[k].pos=temp;
			spheres[k].radius=r;
			spheres[k].name="Sph" +k as string;
			
			spheres[k].name+="_Weight="+weight as string;
			spheres[k].segments=30;
			spheres[k].wireColor=random [0,0,0][255,255,255];
			k+=1;
		)
		close in_file;
 )

free spheres; -- free memory