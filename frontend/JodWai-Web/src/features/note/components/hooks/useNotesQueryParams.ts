import {
  NoteSortBy,
  NoteSortOrder,
  type SortBy,
  type SortOrder,
} from "../../types/note";

export interface NoteQueryParams {
  search: string;
  createdAfter: string;
  createdBefore: string;
  sortBy: SortBy;
  sortOrder: SortOrder;
  page: number;
  pageSize: number;
}

const DEFAULTS: NoteQueryParams = {
  search: "",
  createdAfter: "",
  createdBefore: "",
  sortBy: NoteSortBy.UpdatedAt,
  sortOrder: NoteSortOrder.Desc,
  page: 1,
  pageSize: 20,
};

export function parseNoteQueryParams(search: string): NoteQueryParams {
  const params = new URLSearchParams(search);

  const page = Number(params.get("page"));
  const pageSize = Number(params.get("pageSize"));

  return {
    search: params.get("search") ?? DEFAULTS.search,
    createdAfter: params.get("createdAfter") ?? DEFAULTS.createdAfter,
    createdBefore: params.get("createdBefore") ?? DEFAULTS.createdBefore,
    sortBy: (params.get("sortBy") as SortBy) || DEFAULTS.sortBy,
    sortOrder: (params.get("sortOrder") as SortOrder) || DEFAULTS.sortOrder,
    page: Number.isFinite(page) && page > 0 ? page : DEFAULTS.page,
    pageSize:
      Number.isFinite(pageSize) && pageSize > 0 ? pageSize : DEFAULTS.pageSize,
  };
}

export function buildNoteQueryString(state: NoteQueryParams): string {
  const params = new URLSearchParams();

  if (state.search) params.set("search", state.search);
  if (state.createdAfter) params.set("createdAfter", state.createdAfter);
  if (state.createdBefore) params.set("createdBefore", state.createdBefore);
  if (state.sortBy !== DEFAULTS.sortBy) params.set("sortBy", state.sortBy);
  if (state.sortOrder !== DEFAULTS.sortOrder)
    params.set("sortOrder", state.sortOrder);
  if (state.page !== DEFAULTS.page) params.set("page", String(state.page));
  if (state.pageSize !== DEFAULTS.pageSize)
    params.set("pageSize", String(state.pageSize));

  //   params.set("search", state.search);
  //   params.set("createdAfter", state.createdAfter);
  //   params.set("createdBefore", state.createdBefore);
  //   params.set("sortBy", state.sortBy);
  //   params.set("sortOrder", state.sortOrder);
  //   params.set("page", String(state.page));
  //   params.set("pageSize", String(state.pageSize));

  return params.toString();
}
