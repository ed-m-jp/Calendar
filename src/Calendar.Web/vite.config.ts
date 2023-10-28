import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import mkcert from 'vite-plugin-mkcert'

export default defineConfig({
    plugins: [plugin(), , mkcert()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    build: {
        rollupOptions: {
            output: {
                assetFileNames: `assets/[name].[hash].[ext]`
            }
        }
    },
    server: {
        proxy: {
            '^/api': {
                target: 'https://localhost:7013/',
                secure: false
            }
        },
        port: 5173
    },
})
