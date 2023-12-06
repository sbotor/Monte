import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ClockService {
  public today() {
    return new Date(new Date().toDateString());
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
