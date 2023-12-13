import { Component, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { ChartsParamsService } from '../charts-params.service';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-cpu-core-select',
  standalone: true,
  imports: [CommonModule, MatInputModule, MatSelectModule,],
  templateUrl: './cpu-core-select.component.html',
  styleUrl: './cpu-core-select.component.scss',
})
export class CpuCoreSelectComponent {
  public readonly chartType = this.params.chartType;
  public readonly cpuCore = computed(() => {
    const params = this.params.paramMap();
    return params.cpuCore || null;
  });

  constructor(private readonly params: ChartsParamsService) {}

  public onChanges(change: MatSelectChange) {
    const value = change.value as number | null;
    this.params.setCpuCore(value);
  }
}
