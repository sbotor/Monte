import { ApexAxisChartSeries, ApexChart, ApexTheme, ApexXAxis, ApexYAxis } from "ng-apexcharts";

export interface ChartOptions {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xAxis: ApexXAxis;
  yAxis: ApexYAxis;
  theme: ApexTheme;
}
