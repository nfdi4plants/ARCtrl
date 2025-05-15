/// <reference types="vitest/config" />
import { defineConfig } from 'vite';
 
export default defineConfig({    
    test: {
        globals: true,
        include : ['Main.fs.ts', '*.test.ts'],
        testTimeout: 1_000_000,
    }
  });