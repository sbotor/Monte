// import { Component, OnDestroy, OnInit, signal } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { ChartsService } from '@features/charts/charts.service';
// import { Subject, takeUntil } from 'rxjs';
// import { ActivatedRoute } from '@angular/router';
// import { ChartDatePickerComponent } from '@features/charts/chart-date-picker/chart-date-picker.component';
// import { NgApexchartsModule } from 'ng-apexcharts';
// import { SpinnerComponent } from '@components/spinner';
// import { MatSidenavModule } from '@angular/material/sidenav';
// import { MatCardModule } from '@angular/material/card';

// @Component({
//   selector: 'app-avg-cpu-usage',
//   standalone: true,
//   imports: [
//     CommonModule,
//     ChartDatePickerComponent,
//     NgApexchartsModule,
//     SpinnerComponent,
//     MatSidenavModule,
//     MatCardModule,
//   ],
//   templateUrl: './avg-cpu-usage.component.html',
//   styleUrl: './avg-cpu-usage.component.scss',
// })
// export class AvgCpuUsageComponent implements OnInit, OnDestroy {
//   private readonly destroyed$ = new Subject<void>();

//   private _id = '';

//   constructor(
//     private readonly api: ChartsService,
//     private readonly route: ActivatedRoute
//   ) {}

//   ngOnInit(): void {
//     this.route.paramMap.pipe(takeUntil(this.destroyed$)).subscribe((x) => {
//       this._id = x.get('id')!;
//       this.fetchData();
//     });
//   }

//   ngOnDestroy(): void {
//     this.destroyed$.next();
//     this.destroyed$.complete();
//   }

//   private fetchData() {
//     this._isLoading.set(true);
//     this.api
//       .getAvgCpuUsage(this._id, this.range())
//       .pipe(takeUntil(this.destroyed$))
//       .subscribe((data) => {
//         this._chartOptions.update((x) => {
//           x.series[0].data = data.values.map((y, i) => {
//             return { x: data.labels[i].valueOf(), y: y };
//           });

//           return x;
//         });
//         this._isLoading.set(false);
//       });
//   }
// }
