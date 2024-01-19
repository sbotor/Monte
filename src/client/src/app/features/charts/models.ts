import { ApexAxisChartSeries, ApexChart, ApexTheme, ApexXAxis, ApexYAxis } from "ng-apexcharts";

export interface ChartOptions {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xAxis: ApexXAxis;
  yAxis: ApexYAxis;
  theme: ApexTheme;
}

export type ChartType = 'cpuUsage' | 'memoryUsage' | 'cpuLoad' | 'memoryAvailable';

export interface SelectOption<T> {
  value: T;
  label: string;
}
