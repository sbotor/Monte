import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MachinesService } from '../../features/machines/machines.service';
import { SpinnerComponent } from '../../core/spinner/spinner.component';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-machine-list',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, MatCardModule],
  templateUrl: './machine-list.component.html',
  styleUrl: './machine-list.component.scss'
})
export class MachineListComponent {
  public readonly machines$ = this.api.getMachines();

  constructor(private readonly api: MachinesService) {
  }
}
