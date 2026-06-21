import { EditorContent, useEditor } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import { useState } from "react";
import type {
  CreateNoteRequest,
  NoteDto,
  UpdateNoteRequest,
} from "../types/note";

interface NoteFormProps {
  note?: NoteDto;
  onCreate: (request: CreateNoteRequest) => Promise<void>;
  onUpdate: (request: UpdateNoteRequest) => Promise<void>;
  onClose: () => void;
}

export default function NoteForm({
  note,
  onCreate,
  onUpdate,
  onClose,
}: Readonly<NoteFormProps>) {
  const [title, setTitle] = useState(note?.title ?? "");

  const editor = useEditor({
    extensions: [StarterKit],
    content: note?.content || "<p></p>",
    editorProps: {
      attributes: {
        class:
          "prose max-w-none focus:outline-none min-h-[150px] p-3 border rounded-md bg-white",
      },
    },
  });

  // useEffect(() => {
  //   if (editor && note) {
  //     editor.commands.setContent(note.content);
  //   }
  // }, [editor, note?.content]);

  const handleSave = async () => {
    if (!editor) {
      return;
    }

    if (!title.trim()) {
      alert("Please enter a title");
      return;
    }

    const content = editor.getHTML();

    if (note) {
      await onUpdate({
        id: note.id,
        title,
        content,
      });
    } else {
      await onCreate({
        title,
        content,
      });
    }
  };

  return (
    <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-2xl p-6 flex flex-col gap-4">
        <h2 className="text-xl font-bold text-gray-800">
          {note ? "Edit Note" : "Create New Note"}
        </h2>

        <input
          type="text"
          placeholder="Note Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full text-lg font-semibold p-2 border-b border-gray-300 focus:outline-none focus:border-blue-500"
        />

        {/* Minimal Toolbar */}
        <div className="flex gap-2 p-1 bg-gray-100 rounded border">
          <button
            type="button"
            onClick={() => editor?.chain().focus().toggleBold().run()}
            className={`px-2 py-1 rounded text-xs font-bold ${editor?.isActive("bold") ? "bg-blue-500 text-white" : "hover:bg-gray-200"}`}
          >
            B
          </button>
          <button
            type="button"
            onClick={() => editor?.chain().focus().toggleItalic().run()}
            className={`px-2 py-1 rounded text-xs italic ${editor?.isActive("italic") ? "bg-blue-500 text-white" : "hover:bg-gray-200"}`}
          >
            I
          </button>
        </div>

        <EditorContent editor={editor} />

        <div className="flex justify-end gap-3 mt-4">
          <button
            type="button"
            onClick={onClose}
            className="px-4 py-2 text-sm bg-gray-200 hover:bg-gray-300 text-gray-700 rounded-md transition"
          >
            Cancel
          </button>
          <button
            type="button"
            onClick={handleSave}
            className="px-4 py-2 text-sm bg-blue-600 hover:bg-blue-700 text-white rounded-md transition"
          >
            Save Note
          </button>
        </div>
      </div>
    </div>
  );
}
