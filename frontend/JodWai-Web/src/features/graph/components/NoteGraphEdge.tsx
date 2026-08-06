import { BaseEdge, getBezierPath, type EdgeProps } from "@xyflow/react";
import type { GraphEdge } from "../types";

export default function NoteGraphEdge(props: EdgeProps<GraphEdge>) {
  const {
    id,
    sourceX,
    sourceY,
    targetX,
    targetY,
    sourcePosition,
    targetPosition,
    markerEnd,
    data,
  } = props;

  const color = data?.color ?? "#888";
  const curvature = data?.curvature ?? 0.25;

  const [path] = getBezierPath({
    sourceX,
    sourceY,
    targetX,
    targetY,
    sourcePosition,
    targetPosition,
    curvature,
  });

  return (
    <BaseEdge
      id={id}
      path={path}
      markerEnd={markerEnd}
      style={{
        stroke: color,
        strokeWidth: 2,
      }}
    />
  );
}
