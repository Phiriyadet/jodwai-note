import {
  NoteSortBy,
  NoteSortOrder,
  type SortBy,
  type SortOrder,
} from "../../types/note";

type SortSectionProps = {
  sortBy: SortBy;
  sortOrder: SortOrder;
  onSortByChange: (value: SortBy) => void;
  onSortOrderChange: (value: SortOrder) => void;
};

export function SortSection({
  sortBy,
  sortOrder,
  onSortByChange,
  onSortOrderChange,
}: SortSectionProps) {
  return (
    <div className="flex flex-wrap items-end gap-3">
      <div className="flex flex-col">
        <label htmlFor="sortBy" className="mb-1 text-sm font-medium">
          Sort By
        </label>

        <select
          id="sortBy"
          value={sortBy}
          onChange={(e) => onSortByChange(e.target.value as SortBy)}
          className="rounded-md border border-gray-300 px-3 py-2"
        >
          <option value={NoteSortBy.UpdatedAt}>Updated At</option>
          <option value={NoteSortBy.CreatedAt}>Created At</option>
          <option value={NoteSortBy.Title}>Title</option>
        </select>
      </div>

      <div className="flex flex-col">
        <label htmlFor="sortOrder" className="mb-1 text-sm font-medium">
          Order
        </label>

        <select
          id="sortOrder"
          value={sortOrder}
          onChange={(e) => onSortOrderChange(e.target.value as SortOrder)}
          className="rounded-md border border-gray-300 px-3 py-2"
        >
          <option value={NoteSortOrder.Desc}>Descending</option>
          <option value={NoteSortOrder.Asc}>Ascending</option>
        </select>
      </div>
    </div>
  );
}
