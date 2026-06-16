import { useMutation, useQueryClient } from "@tanstack/react-query";
import { noteApi } from "../api/noteApi";
import { noteKeys } from "./noteKeys";

export function useCreateNoteMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: noteApi.create,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: noteKeys.all,
      });
    },
  });
}
