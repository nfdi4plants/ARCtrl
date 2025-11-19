#!/usr/bin/env bash

# ARCtrl Setup

echo "Restore .NET tools"
dotnet tool restore

echo "Install JavaScript Dependencies"
npm i

echo "Setup Python Virtual Environment"
python -m venv .venv

echo "Install Python Dependencies"
.venv/bin/python -m pip install -U pip setuptools
.venv/bin/python -m pip install uv
.venv/bin/python -m uv pip install -r pyproject.toml --group dev
echo "DONE!"