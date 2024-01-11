import { HttpParams } from "@angular/common/http";

export const cleanParams = (params?: any) => {
  if (params) {
    let httpParams = new HttpParams();

    Object.keys(params).forEach(key => {
      const value = params[key];

      if (value !== undefined && value !== null) {
        httpParams = httpParams.append(key, value);
      }
    });

    params = httpParams;
  }

  return params;
}
