import { DateRange } from '@features/charts/charts.service';

type SerializedDateRange = {
  [K in keyof DateRange]: string;
};

export class UtcDate {
  constructor(private readonly date: Date) {}

  public toString() {
    return new Date(
      this.date.getTime() - this.date.getTimezoneOffset() * 60_000
    ).toISOString();
  }

  public static serializeDateRange(range: DateRange): SerializedDateRange {
    return {
      dateFrom: new UtcDate(range.dateFrom).toString(),
      dateTo: new UtcDate(range.dateTo).toString(),
    };
  }
}
