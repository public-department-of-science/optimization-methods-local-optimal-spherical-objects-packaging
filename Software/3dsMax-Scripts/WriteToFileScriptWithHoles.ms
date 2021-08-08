file_name = "D:\\CoordinateWithHoles.txt";
in_file = openFile file_name

ballsCount = readValue in_file as integer; -- 
holesCount = readValue in_file as integer; --
close in_file
out_name = ("D://"+"ChangedCoordinateWithHolesTest.txt");
out_file = createfile out_name;

format "% "  ballsCount to:out_file;
format "%\n"  holesCount to:out_file;
for v = 1 to ballsCount+1
	do
	(
	format "%" objects[v].pos.x to:out_file;
	format " %" objects[v].pos.y to:out_file;
	format " %" objects[v].pos.z to:out_file;
	format " %\n" objects[v].radius to:out_file;
	)
close out_file