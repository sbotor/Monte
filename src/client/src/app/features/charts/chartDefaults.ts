import { ChartOptions } from './models';

export interface ChartDefaults {
  avgCpuUsage: () => ChartOptions;
}

export const chartDefaults: ChartDefaults = {
  avgCpuUsage: () => {
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
  },
} as const;

export default chartDefaults;
