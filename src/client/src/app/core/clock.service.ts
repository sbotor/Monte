import { Injectable } from '@angular/core';
import { DateRange } from '@features/charts/charts.service';
import { UtcDate } from './utcDate';

@Injectable({
  providedIn: 'root',
})
export class ClockService {
  public today() {
    return new Date(new Date().toDateString());
  }

  public todayRange() {
    const today = this.today();
    return {
      dateFrom: today,
      dateTo: today,
    } as DateRange;
  }

  public addDays(date: Date, count = 1) {
    const newDate = new Date(date.valueOf());
    newDate.setDate(newDate.getDate() + count);
    return newDate;
  }

  public serializeDateRange(range: DateRange) {
    return {
      dateFrom: new UtcDate(range.dateFrom).toString(),
      dateTo: new UtcDate(range.dateTo).toString(),
    };
  }
}
