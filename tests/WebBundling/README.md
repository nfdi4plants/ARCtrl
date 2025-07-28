# Web Bundling Test Project

This project is bundled during js native test task. It's goal is to ensure that the bundling process works correctly for web applications.

# CI

- `npx vite`: to run minimal web page
- `npx vite build`: to build the web page for production (This is tested)

# History

- ARCtrl 3.0.0 prerelease used static imports of `fs/promises` and `path`, which caused issues for web bundling.
