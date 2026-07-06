import { useState } from "react";
import type {
  CreateNoteRequest,
  NoteDto,
  UpdateNoteRequest,
} from "../types/note";
import NoteForm from "./NoteForm";
import NoteItem from "./NoteItem";
import { useUpdateNoteMutation } from "../queries/useUpdateNoteMutation";
import { useCreateNoteMutation } from "../queries/useCreateNoteMutation";
import { useDeleteNoteMutation } from "../queries/useDeleteNoteMutation";
import { useNotesQuery } from "../queries/useNotesQuery";

export default function NoteList() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [currentNote, setCurrentNote] = useState<NoteDto | undefined>(
    undefined,
  );
  const { mutateAsync: createNote } = useCreateNoteMutation();
  const { mutateAsync: updateNote } = useUpdateNoteMutation();
  const { mutateAsync: deleteNote } = useDeleteNoteMutation();
  const { data: notes = [], isLoading, error } = useNotesQuery();

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Failed to load notes</div>;
  }

  const handleCreateNote = async (note: CreateNoteRequest) => {
    const newNote = await createNote(note);
    console.log("New note created:", newNote);
    console.log(newNote.content);
    console.log(typeof newNote.content);

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
    </div>
  );
}
