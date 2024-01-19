import { Injectable, signal } from '@angular/core';
import { ClockService } from '@core/clock.service';
import { ChartOptions } from './models';
import { chartOptionsDefaults, chartParamMapDefaults } from './chartDefaults';
import { BehaviorSubject, map, tap } from 'rxjs';
import { ChartParamMap, ParamsMutator } from './chartParamMap';

export interface ChartDataValues {
  x: number;
  y: number;
}

@Injectable({
  providedIn: 'root',
})
export class ChartsParamsService {
  private readonly _chartOptions = signal<ChartOptions>(chartOptionsDefaults());
  public readonly chartOptions = this._chartOptions.asReadonly();

  private readonly _isLoading = signal(true);
  public readonly isLoading = this._isLoading.asReadonly();

  private readonly _paramMap = new BehaviorSubject<ChartParamMap>(
    chartParamMapDefaults(this.clock)
  );
  public readonly paramMap$ = this._paramMap
    .asObservable()
    .pipe(map((x) => x.get()));

  constructor(private readonly clock: ClockService) {}

  public updateCustomParams(mutator: ParamsMutator) {
    const changed = this._paramMap.value.modify(mutator);
    this._paramMap.next(changed);
  }

  public updateData(values: ChartDataValues[], percents: boolean) {
    this._chartOptions.update((x) => {
      x.series[0].data = values;

      const yAxis = x.yAxis;
      if (percents) {
        yAxis.min = 0;
        yAxis.max = 100;
      } else {
        yAxis.min = undefined
        yAxis.max = undefined
      }

      return x;
    });
  }

  public setLoading(value: boolean) {
    this._isLoading.set(value);
  }
}
