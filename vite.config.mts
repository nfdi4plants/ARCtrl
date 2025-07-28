import { defineConfig } from 'vite';
import path from 'path'
import dts from 'vite-plugin-dts'

export default defineConfig({
    plugins: [
        dts({
            entryRoot: 'src/ARCtrl',
            include: ['src/ARCtrl/**/*.ts', 'src/ARCtrl/**/*.mts'],
        }),
    ],
    build: {
        outDir: 'dist/ts', // Both JS and .d.ts files will go here
        lib: {
          entry: path.resolve(__dirname, 'src/ARCtrl/index.ts'),
          name: 'ARCtrl', // Global variable name if using UMD/IIFE
          fileName: (format) => `arctrl.${format}.js`,
          formats: ['es', 'cjs', 'umd'], // Common formats
        },
        rollupOptions: {
            external: [
                'fable-org/fable-library-js',
                '@nfdi4plants/exceljs',
                'isomorphic-fetch'
            ], // put your external packages here
        }
    },
    test: {
        globals: true,
        include : ['Main.fs.ts', '*.test.ts'],
        testTimeout: 1_000_000,
    }
  });