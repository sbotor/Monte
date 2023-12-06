import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DateRange } from '../charts.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ClockService } from '@core/clock.service';
import { MatNativeDateModule } from '@angular/material/core';

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
  private _prevValues: DateRange = undefined!;

  @Input() initialDateFrom: Date | null = null;
  @Input() initialDateTo: Date | null = null;

  @Output() changed = new EventEmitter<DateRange>();

  constructor(private readonly clock: ClockService) {}

  ngOnInit(): void {
    const today = this.clock.today();

    const values = {
      dateFrom: this.initialDateFrom || today,
      dateTo: this.initialDateTo || this.clock.addDays(today, 1),
    };
    this._prevValues = values;

    this.range.setValue({
      start: values.dateFrom,
      end: values.dateTo,
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

    this.changed.emit({
      dateFrom: values.start,
      dateTo: values.end,
    });
  }
}
