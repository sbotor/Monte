import { Injectable } from '@angular/core';
import { ApiService } from '@core/api.service';
import { PagedResponse } from '@core/models';

export interface MachineOverview {
  id: string;
  displayName: string;
  lastHeartbeat: Date;
  created: Date;
}

export interface GetMachinesRequest {
  page: number;
  pageSize: number;
  orderBy?: string;
  orderByDesc: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class MachinesService {

  constructor(private readonly api: ApiService) { }

  public getMachines(query: GetMachinesRequest) {
    return this.api.get<PagedResponse<MachineOverview>>('machines', query);
  }
}
