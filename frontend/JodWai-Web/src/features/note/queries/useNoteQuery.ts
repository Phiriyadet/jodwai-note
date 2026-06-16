import { useQuery } from "@tanstack/react-query";
import { noteKeys } from "./noteKeys";
import { noteApi } from "../api/noteApi";

export function useNoteQuery(id: string) {
  return useQuery({
    queryKey: noteKeys.detail(id),
    queryFn: () => noteApi.getById(id),
    enabled: !!id,
  });
}
