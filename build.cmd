@echo off

dotnet tool restore
dotnet run --project ./build/build.fsproj %*