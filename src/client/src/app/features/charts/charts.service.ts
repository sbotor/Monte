import { Injectable } from '@angular/core';
import { ApiService } from '@core/api.service';
import { ClockService } from '@core/clock.service';

export interface ChartData<T> {
  labels: Date[];
  values: T[];
}

export interface DateRange {
  dateFrom: Date;
  dateTo: Date;
}

@Injectable({
  providedIn: 'root',
})
export class ChartsService {
  constructor(private readonly api: ApiService, private readonly clock: ClockService) {}

  public getAvgCpuUsage(machineId: string, range: DateRange) {
    const params = {
      dateFrom: this.clock.toUtcString(range.dateFrom),
      dateTo: this.clock.toUtcString(range.dateTo),
    }
    return this.api.get<ChartData<number>>(`charts/${machineId}/cpu/avg`, params);
  }
}
