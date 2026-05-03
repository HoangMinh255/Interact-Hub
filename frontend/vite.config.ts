import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react' // hoặc vue tùy bạn dùng

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    // Đảm bảo chỉ định rõ cấu hình HMR cho Vite để không bị nhầm lẫn
    hmr: {
      protocol: 'ws',
      host: 'localhost',
      port: 5173,
    },
    // Nếu bạn có dùng proxy để gọi API C#, hãy chắc chắn nó không chặn WebSocket của Vite
    proxy: {
      '/api': {
        target: 'https://localhost:5001',
        changeOrigin: true,
        secure: false,
      },
      '/notificationHub': { // Cổng SignalR của bạn
        target: 'https://localhost:5000',
        ws: true, // Cho phép proxy WebSockets cho SignalR
        secure: false,
      }
    }
  }
})