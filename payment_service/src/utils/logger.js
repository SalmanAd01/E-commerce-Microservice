// Lightweight logger wrapper; can be replaced with pino/winston later

const level = process.env.LOG_LEVEL || 'info';
const levels = ['error', 'warn', 'info', 'debug'];
const shouldLog = (l) => levels.indexOf(l) <= levels.indexOf(level);

export const logger = {
  error: (...args) => shouldLog('error') && console.error('[error]', ...args),
  warn: (...args) => shouldLog('warn') && console.warn('[warn] ', ...args),
  info: (...args) => shouldLog('info') && console.log('[info] ', ...args),
  debug: (...args) => shouldLog('debug') && console.debug('[debug]', ...args),
};

export default logger;
