export const noteKeys = {
  all: ["notes"] as const,

  detail: (id: string) => [...noteKeys.all, id] as const,

  search: (keyword: string) => [...noteKeys.all, "search", keyword] as const,
};
