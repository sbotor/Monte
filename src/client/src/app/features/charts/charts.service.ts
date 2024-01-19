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

export interface MemoryChartParams extends ChartParams {
  swap: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ChartsService {
  constructor(private readonly api: ApiService, private readonly clock: ClockService) {}

  public getCpuUsage(agentId: string, params: CpuUsageChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/cpu/usage`, params);
  }

  public getCpuLoad(agentId: string, params: ChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/cpu/load`, params);
  }

  public getMemoryUsage(agentId: string, params: MemoryChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/memory/usage`, params);
  }

  public getMemoryAvailable(agentId: string, params: MemoryChartParams) {
    return this.api.get<ChartData<number>>(`charts/${agentId}/memory/available`, params);
  }
}
