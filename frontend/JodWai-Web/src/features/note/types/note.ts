export interface CreateNoteRequest {
  title: string;
  content: string;
}

export interface UpdateNoteRequest {
  id: string;
  title: string;
  content: string;
}

export interface NoteDto {
  id: string;
  title: string;
  content: string;
  linkedNoteIds: string[];
  createdAt: string;
  updatedAt: string;
}
