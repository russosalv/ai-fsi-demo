export interface AppConfig {
  apiUrl: string;
  production: boolean;
}

export const appConfig: AppConfig = {
  apiUrl: 'https://localhost:7086/api',
  production: false
};

// Funzione per aggiornare la configurazione a runtime se necessario
export function updateConfig(config: Partial<AppConfig>): void {
  Object.assign(appConfig, config);
} 