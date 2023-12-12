import { ChartOptions, ChartType } from './models';

export type ChartDefaults = {
  [K in ChartType]: () => ChartOptions;
};

export const chartDefaults: ChartDefaults = {
  averageCpuUsage: () => {
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
  }
} as const;

export default chartDefaults;
