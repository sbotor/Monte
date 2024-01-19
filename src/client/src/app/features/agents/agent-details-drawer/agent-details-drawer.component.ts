import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AgentDetails } from '../agents.service';
import { Router } from '@angular/router';
import { SpinnerComponent } from '@components/spinner';
import { formatMemoryDisplay } from '@core/display';

@Component({
  selector: 'app-agent-details-drawer',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, SpinnerComponent],
  templateUrl: './agent-details-drawer.component.html',
  styleUrl: './agent-details-drawer.component.scss',
})
export class AgentDetailsDrawerComponent {
  @Input() public agent: AgentDetails | null = null;

  @Output() public readonly close = new EventEmitter<'close'>();

  constructor(private readonly router: Router) {}

  public onChartsClicked(agentId: string) {
    this.router.navigate(['agents', agentId, 'chart']);
  }

  public getFrequencyRange() {
    if (this.agent === null) {
      return '';
    }

    const cpu = this.agent.cpu;
    const minGHz = (cpu.minFreq / 1000).toFixed(3);
    const maxGHz = (cpu.maxFreq / 1000).toFixed(3);

    return `${minGHz} GHz - ${maxGHz} GHz`;
  }

  public formatMemory(value: number) {
    return formatMemoryDisplay(value);
  }
}
