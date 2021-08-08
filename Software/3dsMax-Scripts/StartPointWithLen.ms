max select all;
max delete;

arr = #()
spheres = #()
k=1;
in_name = "D:\\StartPointWithWeight.txt";
in_file = openFile in_name
Rad = readValue in_file
a=sphere pos: [0,0,0] radius: Rad segments:60;	
a.name="MainSph";

--assigns a controller:
a.visibility = bezier_float()
--set the current value to semi-visible:
a.visibility.controller.value = 0.5
--animate the value to 0 on frame 100
animate on at time 100 a.visibility.controller.value = 0.0

if in_file != undefined then
(
	while not(eof in_file)
	do
		(
			x = readValue in_file;
			y = readValue in_file;
			z = readValue in_file;
			temp=[x,y,z]; 
			arr[k]=temp;
			r = readValue in_file;
			weight = readValue in_file;
						
		spheres[k]=sphere();
			spheres[k].pos=temp;
			spheres[k].radius=r;
			spheres[k].segments=30;
			spheres[k].name="Sph" +k as string;
			spheres[k].name+="_Weight="+weight as string;
			
			spheres[k].wireColor=random [0,0,0][255,255,255];
			k+=1;
		)	
		close in_file;
 )

file_name = "D:\\MatrixC.txt";
in_C = openFile file_name
circleCount = readValue in_C as integer-- array size
matrixBond= #()
 
if in_C != undefined then
	(
	while not(eof in_C)
	  do
		(
		  for i=1 to (circleCount * circleCount)
			do(
				matrixBond[i]=readValue in_C;
				)
		)
	)
	close in_C;
	
	arrAboveDiag= #() 
	indI=1; -- number of line (number ofball)
	indK=2; -- position of elememnt in Bond matrix
	indJ=1; -- position of element in Above main Diag array
	
while(indI != circleCount)
	do
	(
		if(indI != 1) then
		(
			indK=2+(indI-1)*(circleCount+1);
		)
		 for i=indK to circleCount*indI
			do
			(
				arrAboveDiag[indJ]=matrixBond[i];
				indJ=indJ+1;
			)
		indI= indI+1;
	)
	
	indJ=1; -- position of element in Above main Diag array
 for i=1 to arr.count-1
	do
		(
		for j=i+1 to arr.count
			do(
				if(arrAboveDiag[indJ] !=0)then
				(
					PointA = arr[i];
					PointB = arr[j];
					newLine = Line pos:PointA thickness: (arrAboveDiag[indJ]*0.5 as float);
					addNewSpline newLine
					addKnot newLine 1 #corner #line PointA
					addKnot newLine 1 #corner #line PointB
					updateShape newLine
				)
				indJ+=1;
			)
		)
	 
	 free arr;
 	 free spheres;