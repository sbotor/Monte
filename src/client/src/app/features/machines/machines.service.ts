import { Injectable } from '@angular/core';
import { ApiService } from '../../core/api.service';
import { BehaviorSubject } from 'rxjs';

export interface MachineOverview {
  id: string;
  displayName: string;
  lastHeartbeat: Date;
}

@Injectable({
  providedIn: 'root'
})
export class MachinesService {

  private readonly gettingMachines = new BehaviorSubject<boolean>(false);

  constructor(private readonly api: ApiService) { }

  public getMachines() {
    return this.api.get<MachineOverview[]>('machines');
  }
}
