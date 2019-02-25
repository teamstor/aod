git pull
git submodule update --remote --merge
MsBuild.exe AOD/AOD.sln /t:Build /p:Configuration=Release
start AOD/bin/AOD.exe
