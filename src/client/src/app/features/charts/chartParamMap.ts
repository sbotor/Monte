import { ChartAggregationType, DateRange } from './charts.service';
import { ChartType } from './models';

export interface ChartParamValues {
  agentId: string;
  dateRange: DateRange;
  chartType: ChartType;
  aggregationType: ChartAggregationType;
  cpuCore?: number | null;
  swapMemory: boolean;
}

export type ParamsMutator = (params: ChartParamValues) => void;

export class ChartParamMap {
  private readonly _params: ChartParamValues;

  constructor(params: ChartParamValues) {
    this._params = this.cloneParams(params);
  }

  public get() {
    return this.cloneParams(this._params);
  }

  public modify(mutator: ParamsMutator) {
    const cloned = this.clone();
    mutator(cloned._params);
    return cloned;
  }

  private clone() {
    return new ChartParamMap(this._params);
  }

  private cloneParams(params: ChartParamValues): ChartParamValues {
    return {
      ...params,
      dateRange: this.cloneDateRange(params.dateRange),
    };
  }

  private cloneDateRange(range: DateRange) {
    return {
      dateFrom: new Date(range.dateFrom),
      dateTo: new Date(range.dateTo),
    };
  }
}
