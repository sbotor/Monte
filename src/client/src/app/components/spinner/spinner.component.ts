import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner'
import { BehaviorSubject, Observable, Subject, distinctUntilChanged, takeUntil } from 'rxjs';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule],
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.scss'
})
export class SpinnerComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  @Input() public loaded: boolean | Observable<boolean> = false;
  @Input() public diameter = 100;

  private readonly _isLoaded = new BehaviorSubject(false);
  public readonly isLoaded$ = this._isLoaded.asObservable().pipe(distinctUntilChanged());

  ngOnInit(): void {
    const observable = this.loaded as Observable<boolean>;
    if (observable) {
      observable.pipe(takeUntil(this.destroyed$)).subscribe(this._isLoaded);
    } else {
      this._isLoaded.next(this.loaded as boolean);
    }
  }

  ngOnDestroy(): void {
      this.destroyed$.next();
      this.destroyed$.complete();
  }
}
