all: clean build test

clean:
	msbuild /t:clean /p:Configuration=Release


build:
	msbuild /t:build /p:Configuration=Release

test:
	MSTest /nologo /testsettings:Local.testsettings /testcontainer:ParallelSelenium\bin\Release\ParallelSelenium.dll