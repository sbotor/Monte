import { Injectable } from '@angular/core';
import { DateRange } from '@features/charts/charts.service';

@Injectable({
  providedIn: 'root'
})
export class ClockService {
  public today() {
    return new Date(new Date().toDateString());
  }

  public todayRange() {
    const today = this.today();
    return {
      dateFrom: today,
      dateTo: today
    } as DateRange;
  }

  public addDays(date: Date, count = 1) {
    const newDate = new Date(date.valueOf());
    newDate.setDate(newDate.getDate() + count);
    return newDate;
  }

  public toUtcString(date: Date) {
    return new Date(date.getTime() - date.getTimezoneOffset() * 60_000).toISOString();
  }
}
