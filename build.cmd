@echo off

dotnet paket restore
cls 
dotnet run --project ./build/build.fsproj %*