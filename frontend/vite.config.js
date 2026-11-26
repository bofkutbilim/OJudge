import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import { routes } from './src/routes.js'

export default defineConfig({
  plugins: [
    react(),
    {
      name: 'onlinejudge-html-pages',
      apply: 'build',
      enforce: 'post',
      generateBundle: {
        order: 'post',
        handler(_, bundle) {
          // Keep bookmarked .html URLs usable on a plain static file server.
          const { source } = bundle['index.html']
          for (const { path } of routes) {
            this.emitFile({ type: 'asset', fileName: path.slice(1), source })
          }
        },
      },
    },
  ],
})
