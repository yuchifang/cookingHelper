import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';

const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ''
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "cookinghelper.client";

if (!certificateName) {
    console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.')
    process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);
console.log("---------------")
console.log(certFilePath)
console.log(keyFilePath)
if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    console.log('Generating certificate using dotnet dev-certs...');
    const result = child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit' });

    if (result.status !== 0) {
        throw new Error("Failed to generate the HTTPS certificate.");
    }
}
// 確保讀取證書內容
const cert = fs.readFileSync(certFilePath);
const key = fs.readFileSync(keyFilePath);
console.log("-------------");
console.log(cert);

console.log(key);
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
            key,
            cert
        }
    }
})
