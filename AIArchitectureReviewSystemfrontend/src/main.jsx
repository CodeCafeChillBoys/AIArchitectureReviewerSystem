import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import { initSignalR } from './services/signalrService'

// Khởi tạo kết nối SignalR ngay khi tải ứng dụng
initSignalR();

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
