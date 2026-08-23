/**
 * Vite 构建配置
 * -------------
 * - 使用官方 Vue 插件。
 * - 配置别名 @ 指向 src，方便导入。
 */
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'node:path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  }
})
