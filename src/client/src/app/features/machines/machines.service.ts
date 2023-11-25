import { Injectable } from '@angular/core';
import { ApiService } from '../../api.service';

export interface MachineOverview {
  id: string;
  displayName: string;
  lastHeartbeat: Date;
}

@Injectable({
  providedIn: 'root'
})
export class MachinesService {

  constructor(private readonly api: ApiService) { }

  public getMachines() {
    return this.api.get<MachineOverview[]>('machines');
  }
}
