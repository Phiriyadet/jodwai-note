import { memo } from "react";
import { Handle, Position, type NodeProps } from "@xyflow/react";

import type { GraphNode } from "../types";

function NoteGraphNode({ data }: NodeProps<GraphNode>) {
  return (
    <div
      className="w-[180px] max-w-[220px] rounded-lg border border-gray-300 px-4 py-3 text-white shadow"
      style={{ backgroundColor: data.color }}
    >
      <Handle type="target" position={Position.Top} />
      <div className="truncate font-medium">{data.label}</div>
      <Handle type="source" position={Position.Bottom} />
    </div>
  );
}

export default memo(NoteGraphNode);
