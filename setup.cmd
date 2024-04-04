@echo off
TITLE ARCtrl Setup

ECHO Restore .NET tools
CALL dotnet tool restore

ECHO Install JavaScript Dependencies
REM npm is a batch file itself. Must use with "CALL" otherwise will exit afterwards.
CALL npm i

ECHO Setup Python Virtual Environment
CALL py -m venv .venv

ECHO Install Python Dependencies
CALL .\.venv\Scripts\python.exe -m pip install -U pip setuptools
CALL .\.venv\Scripts\python.exe -m pip install poetry
CALL .\.venv\Scripts\python.exe -m poetry install --no-root
ECHO DONE!