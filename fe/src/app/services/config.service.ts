import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { AppConfig, updateConfig } from '../config/app.config';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private configLoaded = false;

  constructor(private http: HttpClient) { }

  loadConfig(): Observable<AppConfig> {
    if (this.configLoaded) {
      return of({} as AppConfig);
    }

    return this.http.get<AppConfig>('/assets/config.json').pipe(
      tap(config => {
        updateConfig(config);
        this.configLoaded = true;
        console.log('Configurazione caricata:', config);
      }),
      catchError(error => {
        console.warn('Impossibile caricare la configurazione da /assets/config.json, uso quella di default', error);
        this.configLoaded = true;
        return of({} as AppConfig);
      })
    );
  }
} 