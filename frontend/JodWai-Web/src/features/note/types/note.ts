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

export const NoteSortBy = {
  CreatedAt: "createdAt",
  UpdatedAt: "updatedAt",
  Title: "title",
} as const;

export type SortBy = (typeof NoteSortBy)[keyof typeof NoteSortBy];

export const NoteSortOrder = {
  Asc: "asc",
  Desc: "desc",
} as const;

export type SortOrder =
  (typeof NoteSortOrder)[keyof typeof NoteSortOrder];

  

export interface GetNotesRequest {
  page?: number;
  pageSize?: number;

  search?: string;
  tag?: string;

  createdAfter?: string;
  createdBefore?: string;

  sortBy?: SortBy;
  sortOrder?: SortOrder;
}

export interface PagedResponse<T> {
  items: T[];

  page: number;
  pageSize: number;

  totalItems: number;
  totalPages: number;
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
