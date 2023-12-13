import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartsService } from '@features/charts/charts.service';
import { ActivatedRoute } from '@angular/router';
import { ChartsParamsService } from '@features/charts/charts-params.service';
import { ChartDatePickerComponent } from '@features/charts/chart-date-picker/chart-date-picker.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { SpinnerComponent } from '@components/spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import { Subject, takeUntil } from 'rxjs';
import { MatDividerModule } from '@angular/material/divider';
import { CpuCoreSelectComponent } from '@features/charts/cpu-core-select/cpu-core-select.component';

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
  ],
  templateUrl: './resource-chart.component.html',
  styleUrl: './resource-chart.component.scss',
})
export class ResourceChartComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  public readonly chartOptions = this.params.chartOptions;
  public readonly isLoading = this.params.isLoading;
  public readonly chartType = this.params.chartType;

  private _id = '';

  constructor(
    private readonly api: ChartsService,
    private readonly route: ActivatedRoute,
    private readonly params: ChartsParamsService
  ) {}

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntil(this.destroyed$)).subscribe((x) => {
      this._id = x.get('id')!;
      this.fetchData();
    });

    this.params.changed$
      .pipe(takeUntil(this.destroyed$))
      .subscribe(() => this.fetchData());
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  private fetchData() {
    this.params.setLoading(true);

    this.api
      .getAvgCpuUsage(this._id, this.params.dateRange())
      .pipe(takeUntil(this.destroyed$))
      .subscribe((data) => {
        this.params.updateData((x) => {
          x[0].data = data.values.map((y, i) => {
            return { x: data.labels[i].valueOf(), y: y };
          });
        });

        this.params.setLoading(false);
      });
  }
}
