/// <reference types="vitest/config" />
import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts'
 
export default defineConfig({
    plugins: [
        dts({
            include: ['src/ARCtrl'],
            tsconfigPath: 'tsconfig.json',
        })
    ],
    test: {
        globals: true,
        include : ['Main.fs.ts', '*.test.ts'],
        testTimeout: 1_000_000,
    }
  });