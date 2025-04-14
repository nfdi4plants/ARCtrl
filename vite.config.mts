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
    build: {
        lib: {
            entry: './src/ARCtrl/index.js', // Entry file for your library
            name: "@nfdi4plants/arctrl",
            fileName: (format) => `index.${format}.js`,
        },
        rollupOptions: {
            // Exclude peer dependencies from the final bundle
            external: ['@fable-org/fable-library-js']
        },
        outDir : 'dist/ts'
    },
    test: {
        globals: true,
        //'tests/**/js/Main.fs.ts',
        include: [ 'tests/JavaScript/*.test.js', 'tests/JavaScript/*.test.ts'],
    }
  });