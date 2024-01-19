import { Injectable } from '@angular/core';
import { ChartsParamsService } from './charts-params.service';
import {
  ChartData,
  ChartParams,
  ChartsService,
  CpuUsageChartParams,
  MemoryChartParams,
} from './charts.service';
import { UtcDate } from '@core/utcDate';
import { Observable, of, switchMap } from 'rxjs';
import { ChartParamValues } from './chartParamMap';

@Injectable({
  providedIn: 'root',
})
export class ChartsDataService {
  constructor(
    private readonly params: ChartsParamsService,
    private readonly api: ChartsService
  ) {}

  public fetchData(params: ChartParamValues): Observable<boolean> {
    switch (params.chartType) {
      case 'cpuUsage':
        return this.fetchCpuUsage(params);
      case 'memoryUsage':
        return this.fetchMemoryUsage(params);
      case 'cpuLoad':
        return this.fetchCpuLoad(params);
      case 'memoryAvailable':
        return this.fetchMemoryAvailable(params);
    }
  }

  private fetchCpuUsage(params: ChartParamValues) {
    const request: CpuUsageChartParams = {
      ...this.getBaseParams(params),
      core: params.cpuCore,
    };

    return this.api.getCpuUsage(params.agentId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x, true);
        return of(true);
      })
    );
  }

  private fetchCpuLoad(params: ChartParamValues) {
    const request: CpuUsageChartParams = {
      ...this.getBaseParams(params),
    };

    return this.api.getCpuLoad(params.agentId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x, false);
        return of(true);
      })
    );
  }

  private fetchMemoryUsage(params: ChartParamValues) {
    const request: MemoryChartParams = {
      ...this.getBaseParams(params),
      swap: params.swapMemory,
    };

    return this.api.getMemoryUsage(params.agentId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x, true);
        return of(true);
      })
    );
  }

  private fetchMemoryAvailable(params: ChartParamValues) {
    const request: MemoryChartParams = {
      ...this.getBaseParams(params),
      swap: params.swapMemory,
    };

    return this.api.getMemoryAvailable(params.agentId, request).pipe(
      switchMap((x) => {
        this.setNewValues(x, false);
        return of(true);
      })
    );
  }

  private setNewValues(data: ChartData<number>, percents: boolean) {
    this.params.updateData(
      data.values.map((x, i) => {
        const xVal = data.labels[i].valueOf();
        return { x: xVal, y: x };
      }),
      percents
    );
  }

  private getBaseParams(params: ChartParamValues): ChartParams {
    return {
      ...UtcDate.serializeDateRange(params.dateRange),
      aggregationType: params.aggregationType,
    };
  }
}
