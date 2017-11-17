import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import 'rxjs/add/observable/of';
import { environment } from '../../environments/environment';
import 'rxjs/Rx';

const URL = environment.apiBaseUrl;

@Injectable()
export class ValuesService {

  constructor(private http: HttpClient, private router: Router) { }

  getValues() {
    const headers = this.createAuthorizationHeader();
    return this.http.get(`${URL}/values`)     
      .catch((err: Response) => this.handleError(err));
  }

  private handleError(err: any) {
    if (err.status === 401) {
      this.router.navigate(['login']);
    }
    let resp = err.json();
    if(resp.message){
        err.message = resp.message;
    }
    return Observable.throw(err);
  }

  private createAuthorizationHeader(): Headers {
    const token = localStorage.getItem('access_token');
    const headers = new Headers({ 'Accept': 'application/json' });
    if(token){
        headers.append('Authorization', 'Bearer ' + token);
    }
    return headers;
  }
}
