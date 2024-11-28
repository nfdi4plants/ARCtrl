import asyncio
import os
import shutil
from pathlib import Path
import aiofiles

# Check if a directory exists
async def directory_exists(path):
    return Path(path).is_dir()

# Create a directory
async def create_directory(path):
    Path(path).mkdir(parents=True, exist_ok=True)

# Ensure a directory exists
async def ensure_directory(path):
    if not await directory_exists(path):
        await create_directory(path)

# Ensure the directory for a file exists
async def ensure_directory_of_file(file_path):
    dir_path = Path(file_path).parent
    await ensure_directory(dir_path)

# Check if a file exists
async def file_exists(path):
    return Path(path).is_file()

# Get subdirectories in a directory combined with the input path
async def get_subdirectories(path):
    return [str(entry) for entry in Path(path).iterdir() if entry.is_dir()]

# Get the path of all files in a directory combined with the input path
async def get_subfiles(path):
    return [str(entry) for entry in Path(path).iterdir() if entry.is_file()]

# Move a file
async def move_file(old_path, new_path):
    shutil.move(old_path, new_path)

# Move a directory
async def move_directory(old_path, new_path):
    shutil.move(old_path, new_path)

# Delete a file
async def delete_file(path):
    try:
        os.remove(path)
    except FileNotFoundError:
        pass

# Delete a directory (and its contents)
async def delete_directory(path):
    shutil.rmtree(path, ignore_errors=True)

# Read file as text
async def read_file_text(path):
    async with aiofiles.open(path, mode='r', encoding='utf-8') as f:
        return await f.read()

# Read file as binary
async def read_file_binary(path):
    async with aiofiles.open(path, mode='rb') as f:
        return await f.read()

# Write text to a file
async def write_file_text(path, text):
    async with aiofiles.open(path, mode='w', encoding='utf-8') as f:
        await f.write(text)

# Write binary data to a file
async def write_file_binary(path, bytes_data):
    async with aiofiles.open(path, mode='wb') as f:
        await f.write(bytes_data)

# Example usage
# asyncio.run(ensure_directory('test_dir'))
