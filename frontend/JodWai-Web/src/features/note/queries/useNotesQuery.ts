import { useQuery } from "@tanstack/react-query";
import { noteApi } from "../api/noteApi";
import type { GetNotesRequest } from "../types/note";
import { noteKeys } from "./noteKeys";

export function useNotesQuery(request: GetNotesRequest) {
  return useQuery({
    queryKey: noteKeys.list(request),
    queryFn: () => noteApi.getNotes(request),
  });
}

export function useAllNotesQuery() {
  return useQuery({
    queryKey: noteKeys.graph(),
    queryFn: noteApi.getAllNotes,
    staleTime: 30_000,
  });
}
