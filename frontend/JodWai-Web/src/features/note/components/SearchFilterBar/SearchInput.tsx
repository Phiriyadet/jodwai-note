import { Search, X } from "lucide-react";

type SearchInputProps = {
  value: string;
  placeholder?: string;
  onChange: (value: string) => void;
  onSearch: () => void;
  onClear: () => void;
};

export function SearchInput({
  value,
  placeholder = "Search notes...",
  onChange,
  onSearch,
  onClear,
}: SearchInputProps) {
  return (
    <div className="relative flex-1">
      <Search
        size={18}
        className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400"
      />

      <input
        type="text"
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.target.value)}
        onKeyDown={(e) => {
          if (e.key === "Enter") {
            onSearch();
          }
        }}
        className="
          h-11 w-full rounded-lg border border-gray-300
          bg-white py-2 pl-10 pr-10
          text-sm
          outline-none
          transition
          placeholder:text-gray-400
          focus:border-blue-500
          focus:ring-2
          focus:ring-blue-200
        "
      />

      {value && (
        <button
          type="button"
          onClick={onClear}
          className="
            absolute right-3 top-1/2
            -translate-y-1/2
            rounded-full
            p-1
            text-gray-400
            transition
            hover:bg-gray-100
            hover:text-gray-700
          "
        >
          <X size={16} />
        </button>
      )}
    </div>
  );
}