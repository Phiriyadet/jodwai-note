import { httpClient } from "../../../lib/httpClient";
import type {
  CreateNoteRequest,
  GetNotesRequest,
  NoteDto,
  PagedResponse,
  UpdateNoteRequest,
} from "../types/note";
import { endpoints } from "./endpoints";

const MAX_PAGE_SIZE = 100; 

export const noteApi = {

  // noteApi.ts
  getAllNotes: async (): Promise<NoteDto[]> => {
    const response = await httpClient.get<PagedResponse<NoteDto>>(
      endpoints.notes,
      {
        params: { page: 1, pageSize: MAX_PAGE_SIZE },
      },
    );
  
    return response.data.items;
  },
  
  getNotes: async (
    request: GetNotesRequest,
  ): Promise<PagedResponse<NoteDto>> => {
    const response = await httpClient.get<PagedResponse<NoteDto>>(
      endpoints.notes,
      {
        params: request,
      },
    );

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
