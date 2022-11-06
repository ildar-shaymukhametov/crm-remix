import { getAdminServer } from 'mockttp'

const mockServer = getAdminServer();
mockServer.start().then(() => {
  console.info('🔶 Mock server installed')
});

process.on('SIGINT', () => mockServer.stop());
process.on('SIGTERM', () => mockServer.stop());
