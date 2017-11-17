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
    return this.http.get(`${URL}/values`);          
  }

}
