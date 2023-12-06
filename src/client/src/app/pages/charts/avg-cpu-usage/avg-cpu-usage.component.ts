import {
  Component,
  OnDestroy,
  OnInit,
  Signal,
  WritableSignal,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ChartsService,
  DateRange,
} from '@features/charts/charts.service';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ClockService } from '@core/clock.service';
import { ChartDatePickerComponent } from '@features/charts/chart-date-picker/chart-date-picker.component';
import { NgApexchartsModule } from 'ng-apexcharts';
import { SpinnerComponent } from '@components/spinner';
import { ChartOptions } from '@features/charts/models';

@Component({
  selector: 'app-avg-cpu-usage',
  standalone: true,
  imports: [
    CommonModule,
    ChartDatePickerComponent,
    NgApexchartsModule,
    SpinnerComponent,
  ],
  templateUrl: './avg-cpu-usage.component.html',
  styleUrl: './avg-cpu-usage.component.scss',
})
export class AvgCpuUsageComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly _isLoading = signal(true);
  public readonly isLoading = this._isLoading.asReadonly();

  private readonly _range: WritableSignal<DateRange>;
  public readonly range: Signal<DateRange>;

  private _id = '';

  private readonly _chartOptions = signal<ChartOptions>({
    series: [{ name: 'values', data: [] }],
    chart: { type: 'line', width: 800, height: 500 },
    xAxis: { type: 'datetime', categories: [] },
  });
  public readonly chartOptions = this._chartOptions.asReadonly();

  constructor(
    private readonly api: ChartsService,
    private readonly route: ActivatedRoute,
    private readonly clock: ClockService
  ) {
    const today = this.clock.today();
    this._range = signal<DateRange>({
      dateFrom: today,
      dateTo: this.clock.addDays(today, 1),
    });
    this.range = this._range.asReadonly();
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntil(this.destroyed$)).subscribe((x) => {
      this._id = x.get('id')!;
      this.fetchData();
    });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onDateChange(range: DateRange) {
    this._range.set({ ...range });
    this.fetchData();
  }

  private fetchData() {
    this._isLoading.set(true);
    this.api
      .getAvgCpuUsage(this._id, this.range())
      .pipe(takeUntil(this.destroyed$))
      .subscribe((data) => {
        console.log(data);
        this._chartOptions.update(x => {
          x.series[0].data = data.values;
          x.xAxis.categories = data.labels;
          return x;
        });
        this._isLoading.set(false);
      });
  }
}
