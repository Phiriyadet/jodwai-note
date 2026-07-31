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
