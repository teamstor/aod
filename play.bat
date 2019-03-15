git pull
git submodule update --remote --merge
cd AOD
C:\Windows\Microsoft.Net\Framework\v4.0.30319\MSBuild.exe AOD.sln /t:Build /p:Configuration=Release
start bin/AOD.exe
