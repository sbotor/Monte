import { Injectable } from '@angular/core';
import { ApiService } from '@core/api.service';
import { PagedResponse } from '@core/models';

export interface AgentOverview {
  id: string;
  displayName: string;
  lastHeartbeat: Date;
  created: Date;
}

export interface GetAgentsRequest {
  page: number;
  pageSize: number;
  orderBy?: string;
  orderByDesc: boolean;
}

export interface AgentDetails {
  id: string;
  lastHeartbeat: string;
  displayName: string;
  cpu: AgentCpuDetails;
  memory: AgentMemoryDetails;
}

export interface AgentCpuDetails {
  logicalCount: number;
  physicalCount: number;
  minFreq: number;
  maxFreq: number;
}

export interface AgentMemoryDetails {
  total: number;
  swap: number;
}

@Injectable({
  providedIn: 'root',
})
export class AgentsService {
  constructor(private readonly api: ApiService) {}

  public getAgents(query: GetAgentsRequest) {
    return this.api.get<PagedResponse<AgentOverview>>('agents', query);
  }

  public getAgentDetails(id: string) {
    return this.api.get<AgentDetails>(`agents/${id}`);
  }
}
