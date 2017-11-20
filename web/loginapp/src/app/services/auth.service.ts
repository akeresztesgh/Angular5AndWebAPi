import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/of';
import { environment } from '../../environments/environment';
import 'rxjs/Rx';

const URL = environment.apiBaseUrl;

@Injectable()
export class AuthService {

  constructor(private http: Http) { }

  login(username: string, password: string): Observable<boolean> {
    let headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });
    let body = `username=${username}&password=${password}&grant_type=password`;
    let options = new RequestOptions({ headers: headers });

      return this.http.post(`${URL}/token`, body, options)
      .map((res: Response) => res.json())
      .map((result: any) => {
          if(result){
              localStorage.setItem('access_token', result.access_token);
              localStorage.setItem('refresh_token', result.refresh_token);
              return true;
          }
          // Map returns Observable<type> by default
          return false;
      })
      .catch(() => Observable.of(false));
  }

  logout(): void {
    localStorage.removeItem('access_token');
  }

  getToken() {
    return localStorage.getItem('access_token');
  }

  get isLoggedIn(): boolean {
    let token = localStorage.getItem('access_token');
    return (token || '').length > 0;
  }

  refreshToken(): Observable<string>{
    let refreshToken = localStorage.getItem('refresh_token');
    let body = `grant_type=refresh_token&refresh_token=${refreshToken}`;
    return this.http.post(`${URL}/token`, body)
    .map((res:Response) => res.json())
    .map((result: any) => {
      localStorage.setItem('access_token', result.access_token);
      localStorage.setItem('refresh_token', result.refresh_token);
      return result.access_token;
    });
  }
}
