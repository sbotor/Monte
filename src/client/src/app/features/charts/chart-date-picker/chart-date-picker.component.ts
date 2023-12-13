import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DateRange } from '../charts.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ClockService } from '@core/clock.service';
import { MatNativeDateModule } from '@angular/material/core';
import { ChartsParamsService } from '../charts-params.service';

@Component({
  selector: 'app-chart-date-picker',
  standalone: true,
  imports: [
    CommonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatNativeDateModule,
  ],
  templateUrl: './chart-date-picker.component.html',
  styleUrl: './chart-date-picker.component.scss',
})
export class ChartDatePickerComponent implements OnInit {
  public readonly range = new FormGroup({
    start: new FormControl<Date | null>(null),
    end: new FormControl<Date | null>(null),
  });

  private _prevValues: DateRange = { ...this.params.dateRange() };

  constructor(
    private readonly clock: ClockService,
    private readonly params: ChartsParamsService
  ) {}

  ngOnInit(): void {
    const today = this.clock.today();

    this.range.setValue({
      start: this._prevValues.dateFrom,
      end: this._prevValues.dateTo,
    });
  }

  public onClosed() {
    const values = this.range.value;
    if (values.start === undefined || values.start === null) {
      return;
    }

    if (values.end === undefined || values.end === null) {
      return;
    }

    if (
      values.start === this._prevValues.dateFrom &&
      values.end === this._prevValues.dateTo
    ) {
      return;
    }

    const newValues = {
      dateFrom: values.start,
      dateTo: values.end,
    };
    this._prevValues = newValues;

    this.params.setDateRange(newValues);
  }
}
