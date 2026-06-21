import { httpClient } from "../../../lib/httpClient";
import type {
  CreateNoteRequest,
  NoteDto,
  UpdateNoteRequest,
} from "../types/note";
import { endpoints } from "./endpoints";

export const noteApi = {
  getAll: async (): Promise<NoteDto[]> => {
    const response = await httpClient.get<NoteDto[]>(endpoints.notes);

    return response.data;
  },

  search: async (keyword: string): Promise<NoteDto[]> => {
    const response = await httpClient.get<NoteDto[]>(endpoints.notes, {
      params: {
        keyword,
      },
    });

    return response.data;
  },

  getById: async (id: string): Promise<NoteDto> => {
    const response = await httpClient.get<NoteDto>(endpoints.noteById(id));

    return response.data;
  },

  create: async (request: CreateNoteRequest): Promise<NoteDto> => {
    const response = await httpClient.post<NoteDto>(endpoints.notes, request);

    return response.data;
  },

  update: async (request: UpdateNoteRequest): Promise<NoteDto> => {
    const response = await httpClient.put<NoteDto>(endpoints.notes, request);

    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(endpoints.noteById(id));
  },
};
