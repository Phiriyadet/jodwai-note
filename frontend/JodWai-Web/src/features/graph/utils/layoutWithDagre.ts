import { Position } from "@xyflow/react";
import type { GraphEdge, GraphNode } from "../types";
import dagre from "@dagrejs/dagre";

const NODE_WIDTH = 220;
const NODE_HEIGHT = 80;

const DEFAULT_NODE_SEP = 50;
const DEFAULT_RANK_SEP = 80;

export interface LayoutOptions {
  direction?: "TB" | "BT" | "LR" | "RL";
  nodeSep?: number;
  rankSep?: number;
}

export function layoutWithDagre(
  nodes: GraphNode[],
  edges: GraphEdge[],
  options: LayoutOptions = {},
): GraphNode[] {
  const graph = new dagre.graphlib.Graph();

  graph.setDefaultEdgeLabel(() => ({}));

  graph.setGraph({
    rankdir: options.direction ?? "TB",
    nodesep: options.nodeSep ?? DEFAULT_NODE_SEP,
    ranksep: options.rankSep ?? DEFAULT_RANK_SEP,
  });

  for (const node of nodes) {
    graph.setNode(node.id, {
      width: NODE_WIDTH,
      height: NODE_HEIGHT,
    });
  }

  for (const edge of edges) {
    graph.setEdge(edge.source, edge.target);
  }

  dagre.layout(graph);

  return nodes.map((node) => {
    const position = graph.node(node.id);

    return {
      ...node,
      sourcePosition:
        options.direction === "LR" ? Position.Right : Position.Bottom,
      targetPosition: options.direction === "LR" ? Position.Left : Position.Top,
      position: {
        x: position.x - NODE_WIDTH / 2,
        y: position.y - NODE_HEIGHT / 2,
      },
    };
  });
}
