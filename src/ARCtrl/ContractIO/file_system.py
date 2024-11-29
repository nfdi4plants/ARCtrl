import asyncio
import os
import shutil
from pathlib import Path

# Check if a directory exists
def directory_exists(path):
    return Path(path).is_dir()

# Create a directory
def create_directory(path):
    Path(path).mkdir(parents=True, exist_ok=True)

# Ensure a directory exists
def ensure_directory(path):
    if not directory_exists(path):
        create_directory(path)

# Ensure the directory for a file exists
def ensure_directory_of_file(file_path):
    dir_path = Path(file_path).parent
    ensure_directory(dir_path)

# Check if a file exists
def file_exists(path):
    return Path(path).is_file()

# Get subdirectories in a directory combined with the input path
def get_sub_directories(path):
    return [str(entry) for entry in Path(path).iterdir() if entry.is_dir()]

# Get the path of all files in a directory combined with the input path
def get_sub_files(path):
    return [str(entry) for entry in Path(path).iterdir() if entry.is_file()]

# Move a file
def move_file(old_path, new_path):
    shutil.move(old_path, new_path)

# Move a directory
def move_directory(old_path, new_path):
    shutil.move(old_path, new_path)

# Delete a file
def delete_file(path):
    try:
        os.remove(path)
    except FileNotFoundError:
        pass

# Delete a directory (and its contents)
def delete_directory(path):
    shutil.rmtree(path, ignore_errors=True)

# Read file as text
def read_file_text(path):
    with open(path, 'r', encoding='utf-8') as f:
        return f.read()

# Read file as binary
def read_file_binary(path):
    with open(path, 'rb') as f:
        return f.read()

# Write text to a file
def write_file_text(path, text):
    with open(path, 'w', encoding='utf-8') as f:
        f.write(text)

# Write binary data to a file
def write_file_binary(path, bytes_data):
    with open(path, 'wb') as f:
        f.write(bytes_data)

# Example usage
# asyncio.run(ensure_directory('test_dir'))
