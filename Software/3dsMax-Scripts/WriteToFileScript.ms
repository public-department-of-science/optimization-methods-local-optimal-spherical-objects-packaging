out_name = ("D:/"+"/ChangedCoordinate.txt");
out_file = createfile out_name;

for v = 1 to objects.count 
	do
	(
	format "%" objects[v].pos.x to:out_file;
	format " %" objects[v].pos.y to:out_file;
	format " %" objects[v].pos.z to:out_file;
	format " %\n" objects[v].radius to:out_file;
	)
close out_file