type FilterSectionProps = {
  createdAfter: string;
  createdBefore: string;

  onCreatedAfterChange: (value: string) => void;
  onCreatedBeforeChange: (value: string) => void;

  onApply: () => void;
  onReset: () => void;
};

export function FilterSection({
  createdAfter,
  createdBefore,
  onCreatedAfterChange,
  onCreatedBeforeChange,
  onApply,
  onReset,
}: FilterSectionProps) {
  return (
    <div className="flex flex-wrap items-end gap-3">
      <div className="flex flex-col">
        <label className="mb-1 text-sm font-medium">Created After</label>

        <input
          type="date"
          value={createdAfter}
          onChange={(e) => onCreatedAfterChange(e.target.value)}
          className="rounded-md border px-3 py-2"
        />
      </div>

      <div className="flex flex-col">
        <label className="mb-1 text-sm font-medium">Created Before</label>

        <input
          type="date"
          value={createdBefore}
          onChange={(e) => onCreatedBeforeChange(e.target.value)}
          className="rounded-md border px-3 py-2"
        />
      </div>

      <div className="flex items-end gap-2">
        <button
          type="button"
          onClick={onApply}
          className="
      rounded-md
      bg-blue-600
      px-4 py-2
      text-sm font-medium text-white
      transition-colors
      hover:bg-blue-700
      focus:outline-none
      focus:ring-2
      focus:ring-blue-500
      focus:ring-offset-2
    "
        >
          Apply
        </button>

        <button
          type="button"
          onClick={onReset}
          className="
      rounded-md
      border border-gray-300
      bg-white
      px-4 py-2
      text-sm font-medium text-gray-700
      transition-colors
      hover:bg-gray-100
      focus:outline-none
      focus:ring-2
      focus:ring-gray-400
      focus:ring-offset-2
    "
        >
          Reset
        </button>
      </div>
    </div>
  );
}
