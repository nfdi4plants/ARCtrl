@echo off

dotnet tool restore
cls 
dotnet run --project ./build/build.fsproj %*