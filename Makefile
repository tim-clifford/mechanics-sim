make:
	mcs -pkg:gtk-sharp-3.0 Tests.cs Ball.cs PITest.cs Constants.cs ObjectSystem.cs Structures.cs SimObject.cs Graphics.cs Input.cs Block.cs
clean:
	rm -f Tests.exe