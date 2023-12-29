import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DateRange } from '../charts.service';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { ChartsParamsService } from '../charts-params.service';
import { Subject, takeUntil } from 'rxjs';

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
export class ChartDatePickerComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  public readonly range = new FormGroup({
    start: new FormControl<Date | null>(null),
    end: new FormControl<Date | null>(null),
  });

  private _prevValues: DateRange = null!;

  constructor(private readonly params: ChartsParamsService) {}

  ngOnInit(): void {
    this.params.paramMap$.pipe(takeUntil(this.destroyed$)).subscribe((x) => {
      const value = x.dateRange;
      this.range.setValue({
        start: value.dateFrom,
        end: value.dateTo,
      });
      this._prevValues = { ...value };
    });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
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

    this.params.updateCustomParams((x) => {
      x.dateRange = { ...newValues };
    });
  }
}
