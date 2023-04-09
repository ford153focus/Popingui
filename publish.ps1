Remove-Item bin -Recurse -Force -Verbose
Remove-Item obj -Recurse -Force -Verbose

Measure-Command {
    dotnet publish --output bin/Release/win-x64   --runtime win-x64   --configuration Release -p:PublishReadyToRun=true --self-contained true
}

Measure-Command {
    dotnet publish --output bin/Release/linux-x64 --runtime linux-x64 --configuration Release -p:PublishReadyToRun=true --self-contained true
}
