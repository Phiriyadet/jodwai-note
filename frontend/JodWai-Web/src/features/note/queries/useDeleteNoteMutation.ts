import { useMutation, useQueryClient } from "@tanstack/react-query";
import { noteApi } from "../api/noteApi";
import { noteKeys } from "./noteKeys";

export function useDeleteNoteMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: noteApi.delete,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: noteKeys.all,
      });
    },
  });
}
