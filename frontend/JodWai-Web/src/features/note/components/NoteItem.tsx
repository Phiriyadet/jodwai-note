import type { JSONContent } from "@tiptap/react";
import type { NoteDto } from "../types/note";

type NoteItemProps = {
  note: NoteDto;
  onEdit: (note: NoteDto) => void;
  onDelete: (id: string) => void;
};

export default function NoteItem({
  note,
  onEdit,
  onDelete,
}: Readonly<NoteItemProps>) {
  const getSnippet = (content: JSONContent): string => {
    const extractText = (node?: JSONContent): string => {
      if (!node) return "";

      if (typeof node.text === "string") {
        return node.text;
      }

      return (node.content ?? []).map(extractText).join("");
    };

    const text = extractText(content);

    return text.length > 120 ? `${text.slice(0, 120)}...` : text;
  };

  return (
    <div className="bg-white border border-gray-200 rounded-lg shadow-sm hover:shadow-md transition flex flex-col justify-between p-5 h-56">
      <div>
        <h3 className="text-lg font-bold text-gray-900 mb-2 truncate">
          {note.title}
        </h3>
        <p className="text-gray-600 text-sm line-clamp-4">
          {getSnippet(note.content) || (
            <span className="italic text-gray-400">Empty note</span>
          )}
        </p>
      </div>

      <div className="flex justify-end gap-3 border-t pt-3 mt-4">
        <button
          onClick={() => onEdit(note)}
          className="text-xs font-medium text-blue-600 hover:text-blue-800 transition"
        >
          Edit
        </button>
        <button
          onClick={() => onDelete(note.id)}
          className="text-xs font-medium text-red-600 hover:text-red-800 transition"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
