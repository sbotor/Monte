import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsParamsService } from '../charts-params.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { ChartAggregationType } from '../charts.service';
import { SelectOption } from '../models';
import { map } from 'rxjs';

@Component({
  selector: 'app-aggregation-type-select',
  standalone: true,
  imports: [CommonModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './aggregation-type-select.component.html',
  styleUrl: './aggregation-type-select.component.scss',
})
export class AggregationTypeSelectComponent {
  public readonly options: SelectOption<ChartAggregationType>[] = [
    { value: 'Avg', label: 'Average' },
    { value: 'Min', label: 'Minimum' },
    { value: 'Max', label: 'Maximum' },
  ];

  public readonly aggregationType$ = this.params.paramMap$.pipe(
    map((x) => x.aggregationType)
  );

  constructor(private readonly params: ChartsParamsService) {}

  public onChanges(change: MatSelectChange) {
    const value = <ChartAggregationType>change.value;
    this.params.updateCustomParams((x) => (x.aggregationType = value));
  }
}
