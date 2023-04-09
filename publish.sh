rm -rf bin
rm -rf obj
time dotnet publish --output bin/Release/linux-x64 --runtime linux-x64  --configuration Release -p:PublishReadyToRun=true --self-contained true
time dotnet publish --output bin/Release/win-x64   --runtime win-x64    --configuration Release -p:PublishReadyToRun=true --self-contained true
