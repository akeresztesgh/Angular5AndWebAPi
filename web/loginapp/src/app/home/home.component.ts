import { Component, OnInit } from '@angular/core';
import { ValuesService } from '../services/values.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  values: any;
  values1: any;
  values2; any;
  
  constructor(private valuesService: ValuesService) { }

  ngOnInit() {
    this.valuesService.getValues()
    .subscribe(resp => {
      this.values = resp;
    }, () => {
      debugger;
    });
  }

  multipleCalls(){
    this.valuesService.getValues()
    .subscribe(resp => {
      this.values = resp;
    }, () => {
      debugger;
    });
    this.valuesService.getValues()
    .subscribe(resp => {
      this.values1 = resp;
    }, () => {
      debugger;
    });
    this.valuesService.getValues()
    .subscribe(resp => {
      this.values2 = resp;
    }, () => {
      debugger;
    });

  }
}
