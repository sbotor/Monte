import { Component, OnDestroy, OnInit, ViewChild, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { BehaviorSubject, Subject, takeUntil } from 'rxjs';
import {
  AgentDetails,
  AgentOverview,
  AgentsService,
} from '@features/agents/agents.service';
import { PagingInfo } from '@core/models';
import { SpinnerComponent } from '@components/spinner';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { AgentDetailsDrawerComponent } from '@features/agents/agent-details-drawer/agent-details-drawer.component';

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
  ],
  templateUrl: './agent-list.component.html',
  styleUrl: './agent-list.component.scss',
})
export class AgentListComponent implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  private readonly _agents = new BehaviorSubject<AgentOverview[]>([]);
  public readonly agents$ = this._agents.asObservable();

  private readonly _agentDetails = new BehaviorSubject<AgentDetails | null>(
    null
  );
  public readonly agentDetails$ = this._agentDetails.asObservable();

  private readonly _isLoading = signal(true);
  public readonly isLoading = this._isLoading.asReadonly();

  @ViewChild('sidenav') public readonly sidenav: MatSidenav = null!;

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

  private fetchData() {
    const paging = this.pagingInfo();
    const params = {
      page: paging.page,
      pageSize: paging.pageSize,
      orderByDesc: false,
    };

    this._isLoading.set(true);
    this.api
      .getAgents(params)
      .pipe(takeUntil(this.destroyed$))
      .subscribe((x) => {
        this._agents.next(x.items);
        this._pagingInfo.set({ ...x });

        this._isLoading.set(false);
      });
  }
}
