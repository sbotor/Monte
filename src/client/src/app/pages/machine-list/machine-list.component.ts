import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import {
  MachineOverview,
  MachinesService,
} from '@features/machines/machines.service';
import { PagingInfo } from '@core/models';
import { SpinnerComponent } from '@components/spinner';
import { Router } from '@angular/router';

@Component({
  selector: 'app-machine-list',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, MatTableModule, MatPaginatorModule],
  templateUrl: './machine-list.component.html',
  styleUrl: './machine-list.component.scss',
})
export class MachineListComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly _machines = new BehaviorSubject<MachineOverview[]>([]);
  public readonly machines$ = this._machines.asObservable();

  private readonly _isLoading = signal(true);
  public readonly isLoading = this._isLoading.asReadonly();

  private readonly _pagingInfo = signal<PagingInfo>({
    page: 0,
    pageSize: 10,
    pageCount: 1,
    totalCount: 0,
  });
  public readonly pagingInfo = this._pagingInfo.asReadonly();

  public readonly columns = ['displayName', 'id', 'lastHeartbeat', 'created'];

  constructor(private readonly api: MachinesService, private readonly router: Router) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit(): void {
    this.fetchData();
  }

  public onPageChange(event: PageEvent) {
    this._pagingInfo.update((x) => {
      x.page = event.pageIndex;
      x.pageSize = event.pageSize;
      return x;
    });

    this.fetchData();
  }

  public onClickedRow(id: string) {
    this.router.navigate(['machines', id, 'chart']);
  }

  private fetchData() {
    const paging = this.pagingInfo();
    const params = {
      page: paging.page,
      pageSize: paging.pageSize,
      orderByDesc: false,
    };

    this._isLoading.set(true);
    this.api
      .getMachines(params)
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => {
        this._machines.next(x.items);
        this._pagingInfo.set({ ...x });

        this._isLoading.set(false);
      });
  }
}
