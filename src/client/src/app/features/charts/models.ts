import { ApexAxisChartSeries, ApexChart, ApexTheme, ApexXAxis, ApexYAxis } from "ng-apexcharts";

export interface ChartOptions {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xAxis: ApexXAxis;
  yAxis: ApexYAxis;
  theme: ApexTheme;
}

export type ChartType = 'averageCpuUsage';

export type ChartParamKeys = 'cpuCore';
export type ChartParamMap = {
  [K in ChartParamKeys]?: any
};
