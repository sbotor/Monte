import { Injectable } from '@angular/core';
import { ApiService } from '@core/api.service';
import { ClockService } from '@core/clock.service';

export type ChartAggregationType = 'Avg' | 'Min' | 'Max';

export interface ChartData<T> {
  labels: Date[];
  values: T[];
}

export interface DateRange {
  dateFrom: Date;
  dateTo: Date;
}

export interface ChartParams {
  dateFrom: string;
  dateTo: string;
  aggregationType: ChartAggregationType;
}

export interface CpuUsageChartParams extends ChartParams {
  core?: number | null;
}

export interface MemoryUsageChartParams extends ChartParams {
  swap: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ChartsService {
  constructor(private readonly api: ApiService, private readonly clock: ClockService) {}

  public getCpuUsage(agentId: string, params: CpuUsageChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/cpu`, params);
  }

  public getMemoryUsage(agentId: string, params: MemoryUsageChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/memory`, params);
  }
}
