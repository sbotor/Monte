import { Injectable, Signal, WritableSignal, signal } from '@angular/core';
import { DateRange } from './charts.service';
import { ClockService } from '@core/clock.service';
import { ChartOptions, ChartParamMap, ChartType } from './models';
import chartDefaults from './chartDefaults';
import { ApexAxisChartSeries } from 'ng-apexcharts';
import { Subject } from 'rxjs';

export type SeriesUpdateFn = (s: ApexAxisChartSeries) => void;

@Injectable({
  providedIn: 'root',
})
export class ChartsParamsService {
  private readonly _dateRange: WritableSignal<DateRange>;
  public readonly dateRange: Signal<DateRange>;

  private readonly _chartOptions = signal<ChartOptions>(
    chartDefaults.averageCpuUsage()
  );
  public readonly chartOptions = this._chartOptions.asReadonly();

  private readonly _isLoading = signal(true);
  public readonly isLoading = this._isLoading.asReadonly();

  private readonly _changed$ = new Subject<void>();
  public readonly changed$ = this._changed$.asObservable();

  private readonly _chartType = signal<ChartType>('averageCpuUsage');
  public readonly chartType = this._chartType.asReadonly();

  private readonly _paramMap = signal<ChartParamMap>({});
  public readonly paramMap = this._paramMap.asReadonly();

  constructor(private readonly clock: ClockService) {
    this._dateRange = signal<DateRange>(this.clock.todayRange());
    this.dateRange = this._dateRange.asReadonly();
  }

  public setDateRange(range: DateRange) {
    this._dateRange.set({ ...range });
    this._changed$.next();
  }

  public updateData(updateFn: SeriesUpdateFn) {
    this._chartOptions.update((x) => {
      updateFn(x.series);
      return x;
    });
  }

  public setLoading(value: boolean) {
    this._isLoading.set(value);
  }

  public setCpuCore(core?: number | null) {
    this._paramMap.update(x => {
      x.cpuCore = core;
      return x;
    });
    this._changed$.next();
  }
}
