import { fileURLToPath, URL } from 'node:url';
import { visualizer } from "rollup-plugin-visualizer";
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import commonjs from 'vite-plugin-commonjs'
import dotenv from "dotenv";
if (process.env.NODE_ENV !== 'production') {
  dotenv.config();
}

const certFileContent = process.env.COOKINGHELPER_CERT_PEM;
const keyFileContent = process.env.COOKINGHELPER_CERT_KEY;

if (!certFileContent || !keyFileContent) {
  console.error('Missing certificate or key content in environment variables.');
  process.exit(-1);
}


// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin(), commonjs(), visualizer({
    open: true
  })],
  build: {
    target: 'esnext',

    rollupOptions: {
      output: {
        chunkFileNames: '[name]~[hash:6].js',
        manualChunks(id, module) {
          if (id.includes('@mui')) {

            return "@mui"
          }
          if (id.includes('recharts')) {
            return "recharts"
          }
          if (id.includes('lodash')) {
            return "lodash"
          }
          if (id.includes('node_modules')) {
            return 'vendor';
          }
        }
      },
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    proxy: {
      '^/api.*': {
        target: 'http://localhost:5139',
        secure: false
      }
    },
    port: 5173,
    https: {
      key: keyFileContent,
      cert: certFileContent,

    }
  }
});
