import { useQuery } from "@tanstack/react-query";
import { noteKeys } from "./noteKeys";
import { noteApi } from "../api/noteApi";

export function useNotesQuery() {
  return useQuery({
    queryKey: noteKeys.all,
    queryFn: noteApi.getAll,
  });
}
