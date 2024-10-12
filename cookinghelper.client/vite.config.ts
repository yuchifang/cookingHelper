import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import dotenv from 'dotenv';

dotenv.config();

const certFileContent = process.env.COOKINGHELPER_CERT_PEM;
const keyFileContent = process.env.COOKINGHELPER_CERT_KEY;

if (!certFileContent || !keyFileContent) {
    console.error('Missing certificate or key content in environment variables.')
    process.exit(-1);
}


// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
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
})
