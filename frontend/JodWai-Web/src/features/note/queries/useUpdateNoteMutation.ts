import { useMutation, useQueryClient } from "@tanstack/react-query";
import { noteApi } from "../api/noteApi";
import { noteKeys } from "./noteKeys";

export function useUpdateNoteMutation() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: noteApi.update,

    onSuccess: (updatedNote) => {
      queryClient.invalidateQueries({
        queryKey: noteKeys.all,
      });

      queryClient.setQueryData(noteKeys.detail(updatedNote.id), updatedNote);
    },
  });
}
