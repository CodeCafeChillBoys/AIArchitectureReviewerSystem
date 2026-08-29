import * as signalR from '@microsoft/signalr';
import { notificationService } from './notificationService';

let connection = null;

const PRIMARY_HUB_URL = import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5002/hubs/notifications';
const FALLBACK_HUB_URL = 'http://localhost:5000/hubs/notifications';

console.log('%c🔔 [SignalR Service] Initializing SignalR...', 'color: #2563eb; font-weight: bold;');

const createConnection = (url) => {
  return new signalR.HubConnectionBuilder()
    .withUrl(url, {
      skipNegotiation: false,
      transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
    })
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(signalR.LogLevel.Information)
    .build();
};

export const initSignalR = () => {
  if (connection && (connection.state === signalR.HubConnectionState.Connected || connection.state === signalR.HubConnectionState.Connecting)) {
    return connection;
  }

  connection = createConnection(PRIMARY_HUB_URL);

  // Đăng ký nhận sự kiện từ Hub
  connection.on('ReceiveNotification', (data) => {
    console.log('%c🔔 [SignalR] Received Notification:', 'color: #10b981; font-weight: bold;', data);
    notificationService.addNotification({
      type: data.type || 'ai_review',
      title: data.title || 'AI Analysis Completed',
      message: data.message || 'Diagram architecture review has finished.',
      diagramId: data.diagramId,
      workspaceId: data.workspaceId,
      score: data.score,
    });
  });

  // Bắt đầu kết nối
  connection
    .start()
    .then(() => {
      console.log(`%c✅ [SignalR] Connected successfully to ${PRIMARY_HUB_URL}`, 'color: #10b981; font-weight: bold;');
    })
    .catch((err) => {
      console.warn(`⚠️ [SignalR] Failed connecting to ${PRIMARY_HUB_URL}: ${err.message}. Trying fallback ${FALLBACK_HUB_URL}...`);
      
      // Fallback sang Gateway port 5000 nếu port 5002 không phản hồi
      connection = createConnection(FALLBACK_HUB_URL);
      connection.on('ReceiveNotification', (data) => {
        notificationService.addNotification({
          type: data.type || 'ai_review',
          title: data.title || 'AI Analysis Completed',
          message: data.message || 'Diagram architecture review has finished.',
          diagramId: data.diagramId,
          workspaceId: data.workspaceId,
          score: data.score,
        });
      });
      connection.start()
        .then(() => console.log(`%c✅ [SignalR] Connected successfully via Gateway ${FALLBACK_HUB_URL}`, 'color: #10b981; font-weight: bold;'))
        .catch((e) => console.warn(`⚠️ [SignalR] Gateway fallback also failed: ${e.message}`));
    });

  // Gắn helper test vào window để tiện test trong Console
  if (typeof window !== 'undefined') {
    window.testNotification = (msg = 'AI đã phân tích xong sơ đồ kiến trúc của bạn!') => {
      notificationService.addNotification({
        type: 'ai_review',
        title: 'AI Analysis Completed',
        message: msg,
        score: 9.2,
      });
      console.log('✅ Đã tạo thông báo test thành công trên chuông!');
    };
  }

  return connection;
};

export const getSignalRConnection = () => connection;
