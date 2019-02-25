git pull
git submodule update --remote --merge
C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe AOD/AOD.sln /t:Build /p:Configuration=Release
start AOD/bin/AOD.exe
