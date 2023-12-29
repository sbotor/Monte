import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  MatSlideToggleChange,
  MatSlideToggleModule,
} from '@angular/material/slide-toggle';
import { MatInputModule } from '@angular/material/input';
import { ChartsParamsService } from '../charts-params.service';

@Component({
  selector: 'app-memory-type-toggle',
  standalone: true,
  imports: [CommonModule, MatInputModule, MatSlideToggleModule],
  templateUrl: './memory-type-toggle.component.html',
  styleUrl: './memory-type-toggle.component.scss',
})
export class MemoryTypeToggleComponent {
  constructor(private readonly params: ChartsParamsService) {}

  onChange(change: MatSlideToggleChange) {
    this.params.updateCustomParams((x) => (x.swapMemory = change.checked));
  }
}
