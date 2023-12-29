import { Component, OnDestroy, OnInit, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { ChartsParamsService } from '@features/charts/charts-params.service';
import { ChartDatePickerComponent } from '@features/charts/chart-date-picker/chart-date-picker.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { SpinnerComponent } from '@components/spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import {
  ReplaySubject,
  Subject,
  combineLatest,
  map,
  switchMap,
  takeUntil,
  tap,
} from 'rxjs';
import { MatDividerModule } from '@angular/material/divider';
import { CpuCoreSelectComponent } from '@features/charts/cpu-core-select/cpu-core-select.component';
import {
  MachineDetails,
  MachinesService,
} from '@features/machines/machines.service';
import { MemoryTypeToggleComponent } from '@features/charts/memory-type-toggle/memory-type-toggle.component';
import { ChartsDataService } from '@features/charts/charts-data.service';
import { AggregationTypeSelectComponent } from '@features/charts/aggregation-type-select/aggregation-type-select.component';
import { ChartParamValues } from '@features/charts/chartParamMap';
import { ChartAggregationType } from '@features/charts/charts.service';
import { ChartType } from '@features/charts/models';

@Component({
  selector: 'app-resource-chart',
  standalone: true,
  imports: [
    CommonModule,
    ChartDatePickerComponent,
    NgApexchartsModule,
    SpinnerComponent,
    MatSidenavModule,
    MatCardModule,
    MatDividerModule,
    CpuCoreSelectComponent,
    MemoryTypeToggleComponent,
    AggregationTypeSelectComponent,
  ],
  templateUrl: './resource-chart.component.html',
  styleUrl: './resource-chart.component.scss',
})
export class ResourceChartComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  public readonly chartOptions = this.params.chartOptions;
  public readonly isLoading = this.params.isLoading;

  private readonly _machine = new ReplaySubject<MachineDetails>();
  public readonly data$ = combineLatest([
    this.params.paramMap$,
    this._machine,
  ]).pipe(
    map((x) => {
      return { params: x[0], machine: x[1] };
    })
  );

  constructor(
    private readonly data: ChartsDataService,
    private readonly route: ActivatedRoute,
    private readonly params: ChartsParamsService,
    private readonly machines: MachinesService
  ) {}

  ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        map((x) => x.get('id')!),
        switchMap((x) => this.machines.getMachineDetails(x))
      )
      .subscribe((x) => {
        this.params.updateCustomParams((y) => (y.machineId = x.id));
        this._machine.next(x);
      });

    this.data$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => this.fetchData(x.params));
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  private fetchData(params: ChartParamValues) {
    this.params.setLoading(true);

    this.data
      .fetchData(params)
      .pipe(takeUntil(this.destroyed$))
      .subscribe((_) => this.params.setLoading(false));
  }

  public getChartTitle(params: ChartParamValues) {
    const prefix = this.getChartTitlePrefix(params.aggregationType);
    const suffix = this.getChartTitleSuffix(params.chartType);

    return `${prefix} ${suffix}`;
  }

  public getChartSubtitle(params: ChartParamValues) {
    const core = params.cpuCore;

    if (params.chartType !== 'cpu' || core === null || core === undefined) {
      return undefined;
    }

    return 'Core ' + core.toString();
  }

  private getChartTitlePrefix(aggregationType: ChartAggregationType) {
    switch (aggregationType) {
      case 'Avg':
        return 'Average';
      case 'Min':
        return 'Minimum';
      case 'Max':
        return 'Maximum';
    }
  }

  private getChartTitleSuffix(chartType: ChartType) {
    switch (chartType) {
      case 'cpu':
        return 'CPU usage';
      case 'memory':
        return 'memory usage';
    }
  }
}
