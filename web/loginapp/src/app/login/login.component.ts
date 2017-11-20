import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  username: string = 'admin';
  password: string = 'Admin1234!';
  showErrorMessage = false;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.authService.logout();
  }

  authenticateUser(){
    this.authService.login(this.username, this.password)
    .subscribe(result => {
        if(result){
           this.router.navigateByUrl('home');
       }else {
           this.showErrorMessage = true;
       }
    });
}
}
