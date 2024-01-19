import { ClockService } from '@core/clock.service';
import { ChartOptions } from './models';
import { ChartParamMap } from './chartParamMap';

export const chartOptionsDefaults = (): ChartOptions => {
  return {
    series: [{ name: 'values', data: [] }],
    chart: {
      type: 'line',
      width: '100%',
      height: '100%',
      zoom: { enabled: false },
      toolbar: { show: false },
    },
    xAxis: { type: 'datetime' },
    yAxis: { min: 0, max: 100 },
    theme: { mode: 'dark' },
  };
};

export const chartParamMapDefaults = (clock: ClockService): ChartParamMap => {
  return new ChartParamMap({
    agentId: '',
    dateRange: clock.todayRange(),
    chartType: 'cpuUsage',
    aggregationType: 'Avg',
    cpuCore: null,
    swapMemory: false,
  });
};
