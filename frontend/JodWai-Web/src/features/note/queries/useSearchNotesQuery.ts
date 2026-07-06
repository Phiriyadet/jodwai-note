import { useQuery } from "@tanstack/react-query";
import { noteApi } from "../api/noteApi";
import { noteKeys } from "./noteKeys";

export function useSearchNotesQuery(keyword: string) {
  return useQuery({
    queryKey: noteKeys.search(keyword),
    queryFn: () => noteApi.search(keyword),
    enabled: keyword.trim().length > 0,
  });
}
