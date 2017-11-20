import { Component, OnInit } from '@angular/core';
import { ValuesService } from '../services/values.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  values: any;

  constructor(private valuesService: ValuesService) { }

  ngOnInit() {
    this.valuesService.getValues()
    .subscribe(resp => {
      debugger;
      this.values = resp;
    }, () => {
      debugger;
    });

  }
}
