import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsParamsService } from '../charts-params.service';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ChartType, SelectOption } from '../models';
import { map } from 'rxjs';

@Component({
  selector: 'app-chart-type-select',
  standalone: true,
  imports: [CommonModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './chart-type-select.component.html',
  styleUrl: './chart-type-select.component.scss',
})
export class ChartTypeSelectComponent {
  public readonly options: SelectOption<ChartType>[] = [
    { value: 'cpuUsage', label: 'CPU usage' },
    { value: 'cpuLoad', label: 'CPU load' },
    { value: 'memoryUsage', label: 'Memory usage' },
    { value: 'memoryAvailable', label: 'Memory available' },
  ];

  public readonly chartType$ = this.params.paramMap$.pipe(
    map((x) => x.chartType)
  );

  constructor(private readonly params: ChartsParamsService) {}

  public onChanges(change: MatSelectChange) {
    const value = <ChartType>change.value;
    this.params.updateCustomParams((x) => (x.chartType = value));
  }
}
