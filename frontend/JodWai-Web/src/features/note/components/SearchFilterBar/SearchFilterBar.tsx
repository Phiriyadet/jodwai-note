import { SearchInput } from "./SearchInput";
import { FilterSection } from "./FilterSection";
import type { SortBy, SortOrder } from "../../types/note";
import { SortSection } from "./SortSection";

type SearchFilterBarProps = {
  value: string;
  onChange: (value: string) => void;
  onSearch: () => void;
  onClear: () => void;

  filterCreatedAfter: string;
  filterCreatedBefore: string;
  setFilterCreatedAfter: (value: string) => void;
  setFilterCreatedBefore: (value: string) => void;
  handleApplyFilter: () => void;
  handleResetFilter: () => void;
  filterSortBy: SortBy;
  filterSortOrder: SortOrder;
  setFilterSortBy: (value: SortBy) => void;
  setFilterSortOrder: (value: SortOrder) => void;
};

export function SearchFilterBar({
  value,
  onChange,
  onSearch,
  onClear,
  filterCreatedAfter,
  filterCreatedBefore,
  setFilterCreatedAfter,
  setFilterCreatedBefore,
  handleApplyFilter,
  handleResetFilter,
  filterSortBy,
  filterSortOrder,
  setFilterSortBy,
  setFilterSortOrder,
}: SearchFilterBarProps) {
  return (
    <div
      className="
        flex flex-col gap-4
        rounded-xl border border-gray-200
        bg-white p-4 shadow-sm
      "
    >
      <SearchInput
        value={value}
        onChange={onChange}
        onSearch={onSearch}
        onClear={onClear}
      />

      <FilterSection
        createdAfter={filterCreatedAfter}
        createdBefore={filterCreatedBefore}
        onCreatedAfterChange={setFilterCreatedAfter}
        onCreatedBeforeChange={setFilterCreatedBefore}
        onApply={handleApplyFilter}
        onReset={handleResetFilter}
      />

      <SortSection
        sortBy={filterSortBy}
        sortOrder={filterSortOrder}
        onSortByChange={setFilterSortBy}
        onSortOrderChange={setFilterSortOrder}
      />
    </div>
  );
}
