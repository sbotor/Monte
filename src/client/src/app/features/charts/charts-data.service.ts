import { Injectable } from '@angular/core';
import { ChartsParamsService } from './charts-params.service';
import {
  ChartData,
  ChartParams,
  ChartsService,
  CpuUsageChartParams,
  MemoryUsageChartParams,
} from './charts.service';
import { UtcDate } from '@core/utcDate';
import { of, switchMap } from 'rxjs';
import { ChartParamValues } from './chartParamMap';

@Injectable({
  providedIn: 'root',
})
export class ChartsDataService {
  constructor(
    private readonly params: ChartsParamsService,
    private readonly api: ChartsService
  ) {}

  public fetchData(params: ChartParamValues) {
    switch (params.chartType) {
      case 'cpu':
        return this.fetchCpuUsage(params);
      case 'memory':
        return this.fetchMemoryUsage(params);
    }
  }

  private fetchCpuUsage(params: ChartParamValues) {
    const request: CpuUsageChartParams = {
      ...this.getBaseParams(params),
      core: params.cpuCore,
    };

    return this.api.getCpuUsage(params.machineId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x);
        return of(true);
      })
    );
  }

  private fetchMemoryUsage(params: ChartParamValues) {
    const request: MemoryUsageChartParams = {
      ...this.getBaseParams(params),
      swap: params.swapMemory,
    };

    return this.api.getMemoryUsage(params.machineId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x);
        return of(true);
      })
    );
  }

  private setNewValues(data: ChartData<number>) {
    this.params.updateData(
      data.values.map((x, i) => {
        const xVal = data.labels[i].valueOf();
        return { x: xVal, y: x };
      })
    );
  }

  private getBaseParams(params: ChartParamValues): ChartParams {
    return {
      ...UtcDate.serializeDateRange(params.dateRange),
      aggregationType: params.aggregationType,
    };
  }
}
