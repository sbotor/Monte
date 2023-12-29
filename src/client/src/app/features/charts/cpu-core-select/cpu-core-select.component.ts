import { Component, Input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { ChartsParamsService } from '../charts-params.service';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { map } from 'rxjs';

@Component({
  selector: 'app-cpu-core-select',
  standalone: true,
  imports: [CommonModule, MatInputModule, MatSelectModule],
  templateUrl: './cpu-core-select.component.html',
  styleUrl: './cpu-core-select.component.scss',
})
export class CpuCoreSelectComponent {
  public readonly cpuCore$ = this.params.paramMap$.pipe(
    map((x) => x.cpuCore || this.unselected)
  );

  public readonly unselected = -1;

  @Input() cpuCoreCount = 1;

  constructor(private readonly params: ChartsParamsService) {}

  public onChanges(change: MatSelectChange) {
    const value = <number>change.value;
    this.params.updateCustomParams(
      (x) => (x.cpuCore = value === this.unselected ? null : value)
    );
  }

  listCores() {
    const arr: number[] = [];
    for (let i = 0; i < this.cpuCoreCount; i++) {
      arr.push(i);
    }

    return arr;
  }
}
