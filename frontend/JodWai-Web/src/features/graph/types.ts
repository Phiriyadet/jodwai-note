import type { Edge, Node } from "@xyflow/react";

export interface GraphNodeData extends Record<string, unknown> {
  label: string;
  color: string;
}

export interface GraphEdgeData extends Record<string, unknown> {
  color: string;
  curvature?: number;
}

export type GraphNode = Node<GraphNodeData>;
export type GraphEdge = Edge<GraphEdgeData>;
