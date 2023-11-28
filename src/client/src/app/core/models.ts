export interface PagingInfo {
  page: number;
  pageCount: number;
  pageSize: number;
  totalCount: number;
}

export interface PagedResponse<T> extends PagingInfo {
  items: T[];
}
