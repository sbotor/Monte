import { Component, OnDestroy, OnInit, ViewChild, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { BehaviorSubject, Subject, debounceTime, distinctUntilChanged, fromEvent, takeUntil } from 'rxjs';
import {
  AgentDetails,
  AgentOverview,
  AgentSorting,
  AgentsService,
  GetAgentsRequest,
} from '@features/agents/agents.service';
import { PagingInfo, Sorting } from '@core/models';
import { SpinnerComponent } from '@components/spinner';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { AgentDetailsDrawerComponent } from '@features/agents/agent-details-drawer/agent-details-drawer.component';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { MatSortModule, Sort } from '@angular/material/sort';

interface Data {
  agents: AgentOverview[];
  isLoading: boolean;
}

@Component({
  selector: 'app-agent-list',
  standalone: true,
  imports: [
    CommonModule,
    SpinnerComponent,
    MatTableModule,
    MatPaginatorModule,
    MatSidenavModule,
    AgentDetailsDrawerComponent,
    MatInputModule,
    MatIconModule,
    ReactiveFormsModule,
    MatSortModule
  ],
  templateUrl: './agent-list.component.html',
  styleUrl: './agent-list.component.scss',
})
export class AgentListComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly _data = new BehaviorSubject<Data>({ agents: [], isLoading: true });
  public readonly data$ = this._data.asObservable();

  private readonly _agentDetails = new BehaviorSubject<AgentDetails | null>(
    null
  );
  public readonly agentDetails$ = this._agentDetails.asObservable();

  @ViewChild('sidenav') public sidenav: MatSidenav = null!;

  public readonly searchBar = new FormControl('');
  private textFilter = '';

  private _sorting: Sorting<AgentSorting> = { orderBy: null, orderByDesc: false };
  public sorting: Sort = { active: '', direction: 'asc' };

  private readonly _pagingInfo = signal<PagingInfo>({
    page: 0,
    pageSize: 10,
    pageCount: 1,
    totalCount: 0,
  });
  public readonly pagingInfo = this._pagingInfo.asReadonly();

  public readonly columns = ['displayName', 'id', 'lastHeartbeat', 'created'];

  constructor(private readonly api: AgentsService) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit(): void {
    this.fetchData();

    this.searchBar.valueChanges
      .pipe(takeUntil(this.destroyed$), debounceTime(300), distinctUntilChanged())
      .subscribe(x => {
        this.textFilter = x || '';
        this.fetchData();
      });
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
    if (this._agentDetails.value?.id === id) {
      return;
    }

    this._agentDetails.next(null);
    this.sidenav.open();

    this.api
      .getAgentDetails(id)
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => this._agentDetails.next(x));
  }

  public onSidenavClosed() {
    this._agentDetails.next(null);
  }

  public onSortChange(event: Sort) {
    console.log(event);

    const orderByDesc = event.direction === 'desc';
    let orderBy: AgentSorting | null;

    switch (event.active) {
      case 'displayName':
        orderBy = 'Name';
        break;
      case 'lastHeartbeat':
        orderBy = 'LastHeartbeat';
        break;
      case 'created':
        orderBy = 'Created';
        break;
      default:
        orderBy = null;
        break;
    }

    this._sorting = { orderBy, orderByDesc };
    this.sorting = { ...event };

    this.fetchData();
  }

  private fetchData() {
    const paging = this.pagingInfo();
    const params: GetAgentsRequest = {
      page: paging.page,
      pageSize: paging.pageSize,
      orderByDesc: this._sorting.orderByDesc,
      orderBy: this._sorting.orderBy,
      text: this.textFilter
    };

    this._data.next({ agents: [], isLoading: true });
    this.api
      .getAgents(params)
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => {
        this._pagingInfo.set({ ...x });
        this._data.next({ agents: x.items, isLoading: false });
      });
  }
}
