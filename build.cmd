@echo off

dotnet tool restore
dotnet paket restore
cls 
dotnet run --project ./build/build.fsproj %*