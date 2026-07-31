import { useEffect, useState } from "react";
import {
  NoteSortBy,
  NoteSortOrder,
  type CreateNoteRequest,
  type NoteDto,
  type SortBy,
  type SortOrder,
  type UpdateNoteRequest,
} from "../types/note";
import NoteForm from "./NoteForm";
import NoteItem from "./NoteItem";
import { useUpdateNoteMutation } from "../queries/useUpdateNoteMutation";
import { useCreateNoteMutation } from "../queries/useCreateNoteMutation";
import { useDeleteNoteMutation } from "../queries/useDeleteNoteMutation";
import { useNotesQuery } from "../queries/useNotesQuery";
import { SearchFilterBar } from "./SearchFilterBar/SearchFilterBar";
import Pagination from "./Pagination";
import {
  buildNoteQueryString,
  parseNoteQueryParams,
} from "./hooks/useNotesQueryParams";

export default function NoteList() {
  // ==============================
  // State
  // ==============================

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentNote, setCurrentNote] = useState<NoteDto>();

  const [initialParams] = useState(() =>
    parseNoteQueryParams(window.location.search),
  );

  const [page, setPage] = useState(initialParams.page);
  const [pageSize, setPageSize] = useState(initialParams.pageSize);

  // Draft (ใน Form) — init ให้ตรงกับ applied เพื่อไม่ให้ Form ว่างเปล่าตอน refresh
  const [input, setInput] = useState(initialParams.search);
  const [filterCreatedAfter, setFilterCreatedAfter] = useState(
    initialParams.createdAfter,
  );
  const [filterCreatedBefore, setFilterCreatedBefore] = useState(
    initialParams.createdBefore,
  );
  const [filterSortBy, setFilterSortBy] = useState<SortBy>(
    initialParams.sortBy,
  );
  const [filterSortOrder, setFilterSortOrder] = useState<SortOrder>(
    initialParams.sortOrder,
  );

  // Applied (ใช้ Query)
  const [search, setSearch] = useState(initialParams.search);
  const [createdAfter, setCreatedAfter] = useState(initialParams.createdAfter);
  const [createdBefore, setCreatedBefore] = useState(
    initialParams.createdBefore,
  );
  const [sortBy, setSortBy] = useState<SortBy>(initialParams.sortBy);
  const [sortOrder, setSortOrder] = useState<SortOrder>(
    initialParams.sortOrder,
  );

  // ==============================
  // Query Parameters
  // ==============================
  const request = {
    page,
    pageSize,
    search,
    createdAfter,
    createdBefore,
    sortBy,
    sortOrder,
  };

  // ==============================
  // Mutations
  // ==============================

  const { mutateAsync: createNote } = useCreateNoteMutation();
  const { mutateAsync: updateNote } = useUpdateNoteMutation();
  const { mutateAsync: deleteNote } = useDeleteNoteMutation();

  // ==============================
  // Queries
  // ==============================

  const { data, isLoading, error } = useNotesQuery(request);

  // ==============================
  // Sync State -> URL
  // ==============================
  useEffect(() => {
    const queryString = buildNoteQueryString({
      search,
      createdAfter,
      createdBefore,
      sortBy,
      sortOrder,
      page,
      pageSize,
    });

    const newUrl = queryString
      ? `${window.location.pathname}?${queryString}`
      : window.location.pathname;

    window.history.replaceState(null, "", newUrl);
  }, [search, createdAfter, createdBefore, sortBy, sortOrder, page, pageSize]);

  // ==============================
  // Derived State
  // ==============================

  const notes = data?.items ?? [];

  // ==============================
  // Early Return
  // ==============================

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Failed to load notes</div>;
  }

  // ==============================
  // Search Handlers
  // ==============================

  function handleSearch() {
    setPage(1);
    setSearch(input.trim());
  }

  function handleClear() {
    setInput("");
    setSearch("");
    setPage(1);
  }

  function handleApplyFilter() {
    setCreatedAfter(filterCreatedAfter);
    setCreatedBefore(filterCreatedBefore);

    setSortBy(filterSortBy);
    setSortOrder(filterSortOrder);

    setPage(1);
  }

  function handleResetFilter() {
    // Reset UI
    setFilterCreatedAfter("");
    setFilterCreatedBefore("");
    setFilterSortBy(NoteSortBy.UpdatedAt);
    setFilterSortOrder(NoteSortOrder.Desc);

    // Reset Query
    setCreatedAfter("");
    setCreatedBefore("");
    setSortBy(NoteSortBy.UpdatedAt);
    setSortOrder(NoteSortOrder.Desc);

    setPage(1);
  }

  // ==============================
  // CRUD Handlers
  // ==============================

  const handleCreateNote = async (note: CreateNoteRequest) => {
    const newNote = await createNote(note);

    console.log("New note created:", newNote);

    handleCloseModal();
  };

  const handleUpdateNote = async (note: UpdateNoteRequest) => {
    const updatedNote = await updateNote(note);

    console.log("Note updated:", updatedNote);

    handleCloseModal();
  };

  const handleDeleteNote = async (id: string) => {
    if (!confirm("Are you sure you want to delete this note?")) {
      return;
    }

    await deleteNote(id);
  };

  // ==============================
  // Modal Handlers
  // ==============================

  const handleOpenAddModal = () => {
    setCurrentNote(undefined);
    setIsModalOpen(true);
  };

  const handleOpenEditModal = (note: NoteDto) => {
    setCurrentNote(note);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setCurrentNote(undefined);
  };

  return (
    <div className="min-h-screen bg-gray-50 text-gray-900 p-6 md:p-10">
      <div className="max-w-6xl mx-auto">
        <header className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8 border-b pb-6">
          <div>
            <h1 className="text-3xl font-extrabold text-gray-900 tracking-tight">
              My Scratchpad
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              All notes are safely stored in your local browser storage.
            </p>
            <SearchFilterBar
              // Search
              value={input}
              onChange={setInput}
              onSearch={handleSearch}
              onClear={handleClear}
              // Filter
              filterCreatedAfter={filterCreatedAfter}
              filterCreatedBefore={filterCreatedBefore}
              setFilterCreatedAfter={setFilterCreatedAfter}
              setFilterCreatedBefore={setFilterCreatedBefore}
              handleApplyFilter={handleApplyFilter}
              handleResetFilter={handleResetFilter}
              // Sort
              filterSortBy={filterSortBy}
              filterSortOrder={filterSortOrder}
              setFilterSortBy={setFilterSortBy}
              setFilterSortOrder={setFilterSortOrder}
            />
          </div>
          <button
            onClick={handleOpenAddModal}
            className="bg-blue-600 hover:bg-blue-700 text-white font-semibold px-4 py-2.5 rounded-lg shadow-sm hover:shadow text-sm transition"
          >
            + Add New Note
          </button>
        </header>

        {notes.length === 0 ? (
          <div className="text-center py-20 border-2 border-dashed border-gray-300 rounded-xl bg-white">
            <p className="text-gray-400 font-medium">No notes available yet.</p>
            <button
              onClick={handleOpenAddModal}
              className="mt-3 text-sm text-blue-600 font-semibold hover:underline"
            >
              Create your first note!
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {notes.map((note) => (
              <NoteItem
                key={note.id}
                note={note}
                onEdit={handleOpenEditModal}
                onDelete={handleDeleteNote}
              />
            ))}
          </div>
        )}

        {isModalOpen && (
          <NoteForm
            key={currentNote?.id || "new-note"}
            note={currentNote}
            onCreate={handleCreateNote}
            onUpdate={handleUpdateNote}
            onClose={handleCloseModal}
          />
        )}
      </div>
      <Pagination
        page={page}
        pageSize={pageSize}
        totalPages={data?.totalPages ?? 1}
        totalCount={data?.totalItems ?? 0}
        onPageChange={setPage}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPage(1);
        }}
      />
    </div>
  );
}
