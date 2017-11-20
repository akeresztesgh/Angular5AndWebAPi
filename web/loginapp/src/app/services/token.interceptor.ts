import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpSentEvent, HttpHeaderResponse, HttpProgressEvent, HttpResponse, HttpUserEvent, HttpErrorResponse } from "@angular/common/http";
import { AuthService } from './auth.service';
import { Observable } from 'rxjs/Observable';
import { Router } from '@angular/router';
import { BehaviorSubject } from "rxjs/BehaviorSubject";


// NOTE: this link was a great help
//     https://medium.com/@ryanchenkie_40935/angular-authentication-using-the-http-client-and-http-interceptors-2f9d1540eb8
// and this:
//    https://www.intertech.com/Blog/angular-4-tutorial-handling-refresh-token-with-new-httpinterceptor/
@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private isRefreshingToken: boolean = false;
  tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(public auth: AuthService, private router: Router) {}


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpSentEvent | HttpHeaderResponse | HttpProgressEvent | HttpResponse<any> | HttpUserEvent<any>> {
    return next.handle(this.addToken(req, this.auth.getToken()))
        .catch(error => {

            if (error instanceof HttpErrorResponse) {
                switch ((<HttpErrorResponse>error).status) {
                    case 400:
                        return this.handle400Error(error);
                    case 401:
                        return this.handle401Error(req, next);
                }
            } else {
                return Observable.throw(error);
            }
        });
}

  logoutUser(){
    this.router.navigate(['login']);
    return Observable.of(null);
  }

  handle400Error(error) {
    if (error && error.status === 400 && error.error && error.error.error === 'invalid_grant') {
        // If we get a 400 and the error message is 'invalid_grant', the token is no longer valid so logout.
        return this.logoutUser();
    }

    return Observable.throw(error);
  }

  handle401Error(req: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshingToken) {
        this.isRefreshingToken = true;

        // Reset here so that the following requests wait until the token
        // comes back from the refreshToken call.
        this.tokenSubject.next(null);
        return this.auth.refreshToken()
            .switchMap((newToken: string) => {
                if (newToken) {
                    this.tokenSubject.next(newToken);
                    return next.handle(this.addToken(req, newToken));
                }

                // If we don't get a new token, we are in trouble so logout.
                return this.logoutUser();
            })
            .catch(error => {
                // If there is an exception calling 'refreshToken', bad news so logout.
                return this.logoutUser();
            })
            .finally(() => {
                this.isRefreshingToken = false;
            });
    } else {
        return this.tokenSubject
            .filter(token => token != null)
            .take(1)
            .switchMap(token => {
                debugger;
                return next.handle(this.addToken(req, token));
            });
    }
  } 

  addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({ setHeaders: { Authorization: 'Bearer ' + token }})
  }

}