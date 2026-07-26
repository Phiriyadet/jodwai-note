import { SearchInput } from "./SearchInput";

type SearchFilterBarProps = {
  value: string;
  onChange: (value: string) => void;
  onSearch: () => void;
  onClear: () => void;
};

export function SearchFilterBar({
  value,
  onChange,
  onSearch,
  onClear,
}: SearchFilterBarProps) {
  return (
    <div
      className="
        flex flex-col gap-3
        rounded-xl border border-gray-200
        bg-white
        p-4
        shadow-sm
        md:flex-row
        md:items-center
      "
    >
      <SearchInput
        value={value}
        onChange={onChange}
        onSearch={onSearch}
        onClear={onClear}
      />

      {/* Future Filter Section */}
      {/*
      <FilterButton />
      */}
    </div>
  );
}
