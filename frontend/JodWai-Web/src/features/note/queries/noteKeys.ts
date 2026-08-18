import type { GetNotesRequest } from "../types/note";

export const noteKeys = {
  /**
   * Root key for all note-related queries.
   * Used when invalidating every cached note query.
   *
   * Example:
   * queryClient.invalidateQueries({ queryKey: noteKeys.all });
   */
  all: ["notes"] as const,

  /**
   * Cache key for a single note.
   *
   * Example:
   * ["notes", "123"]
   */
  detail: (id: string) => [...noteKeys.all, id] as const,

  /**
   * Cache key for the note list.
   * The request object is included so each unique combination of
   * pagination, search, filtering and sorting has its own cache entry.
   *
   * Examples:
   * ["notes", { page: 1, pageSize: 20 }]
   * ["notes", { page: 2, pageSize: 20 }]
   * ["notes", { search: "react" }]
   * ["notes", { tag: "backend" }]
   * ["notes", { sortBy: "title", sortOrder: "asc" }]
   */
  list: (request: GetNotesRequest) =>
    [...noteKeys.all, request] as const,

  /**
   * Cache key for the full, unpaginated note collection.
   * Used for views that need every note at once, e.g. the note graph.
   *
   * Example:
   * ["notes", "graph"]
   */
  graph: () => [...noteKeys.all, "graph"] as const,
};