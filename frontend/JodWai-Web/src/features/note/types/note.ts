import type { JSONContent } from "@tiptap/react";

export const EMPTY_DOC = {
  type: "doc",
  content: [{ type: "paragraph" }],
};

export interface CreateNoteRequest {
  title: string;
  content: JSONContent;
}

export interface UpdateNoteRequest {
  id: string;
  title: string;
  content: JSONContent;
}

export interface NoteLinkDto {
  targetId: string;
}

export interface NoteDto {
  id: string;
  title: string;
  content: JSONContent;
  links: NoteLinkDto[];
  tags: string[];
  createdAt: string;
  updatedAt: string;
}
