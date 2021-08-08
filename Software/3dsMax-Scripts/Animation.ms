-- select all objects and delete selected elements
max select all;
max delete;

-- initial arrays
spheres = #()
arr = #()
radius = #()

-- chooise directories; open txt file
in_name = "D:\\IterationFixedRadiusAndCombinedObjects.txt"; 

in_file = openFile in_name;

ballsCount = readValue in_file;
holesCount = readValue in_file;
iterationCount = readValue in_file;
countElement = 0;
k=1
	if in_file != undefined then
	(
		while not(eof in_file)
		do
			(
			x = readValue in_file;
			y = readValue in_file;
			z = readValue in_file;
			r = readValue in_file;
			countElement+=4
			temp = [x,y,z];  
			radius[k] = r;
			arr[k] = temp; 
			if (countElement == (4*ballsCount)) then
				(
					k+=1
					arr[k] = [0,0,0];
					radius[k] = readValue in_file;
					countElement = 0;
				)
				k+=1
			)
	)
	close in_file;
	for i =1 to ballsCount+1
		do
		(
			spheres[i]= sphere();
			spheres[i].pos=[0,0,0];
			spheres[i].radius= 0;
		)
	z=1;
with  animate on 
(
	for q=1 to iterationCount
	do
	(
		at time q(
			if z==1 then
				(
				for d =1 to ballsCount+1
					do 
					(
						spheres[d].pos=arr[d];
						spheres[d].radius=radius[d];
						spheres[d].name="Sph_" + d as string;
						spheres[d].segments=60;
						if(d<=holesCount) then
						(
							spheres[d].wireColor=red;
						)
						else
						(
							spheres[d].wireColor=random [0,0,0][50,255,255];
						)
						if (d == ballsCount + 1) then
						(
							spheres[d].name= "MainSph";
							spheres[d].wireColor=blue;
							--assigns a controller:
							spheres[d].visibility = bezier_float()
							--set the current value to semi-visible:
							spheres[d].visibility.controller.value = 0.1
							--animate the value to 0 on frame 100
							animate on at time 100 spheres[d].visibility.controller.value = 0.3
						)
					)
				)
			else
			(
				for d =1 to ballsCount+1
					do (
						spheres[d].pos=arr[(q-1)*(ballsCount+1)+d];
						spheres[d].radius=radius[(q-1)*(ballsCount+1)+d];
						spheres[d].name="Sph" +k as string;
						spheres[d].segments=60;
						if(d<=holesCount) then
						(
							spheres[d].wireColor=red;
						)
						else
						(
							spheres[d].wireColor=random [0,0,0][50,255,255];
						)
						if (d == ballsCount + 1) then
						(
							spheres[d].name= "MainSph";
							spheres[d].wireColor=blue;
							--assigns a controller:
							spheres[d].visibility = bezier_float()
							--set the current value to semi-visible:
							spheres[d].visibility.controller.value = 0.1
							--animate the value to 0 on frame 100
							animate on at time 100 spheres[d].visibility.controller.value = 0.3
						)
					)
			)
		)
		z+=1
		Print q
	)
)

	free spheres;
	free arr;
	free temp;