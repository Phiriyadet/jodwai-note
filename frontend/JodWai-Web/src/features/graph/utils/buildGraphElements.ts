import type { NoteDto } from "../../note/types/note";
import type { GraphEdge, GraphNode } from "../types";
import { getNodeColor } from "./getNodeColor";

export interface GraphElements {
  nodes: GraphNode[];
  edges: GraphEdge[];
}

export function buildGraphElements(notes: NoteDto[]): GraphElements {
  const noteIds = new Set(notes.map((note) => note.id));
  console.log("raw notes:", notes.map(n => ({ id: n.id, links: n.links })));
  const nodes: GraphNode[] = notes.map((note) => ({
    id: note.id,
    type: "note",
    position: {
      x: 0,
      y: 0,
    },
    data: {
      label: note.title,
      color: getNodeColor(note.id),
    },
  }));

  const nodeColorMap = new Map(nodes.map((node) => [node.id, node.data.color]));

  const edges: GraphEdge[] = [];

  const edgePairs = new Set(
    notes.flatMap((note) =>
      note.links.map((link) => `${note.id}:${link.id}`),
    ),
  );

  for (const note of notes) {
    for (const link of note.links) {
      if (!noteIds.has(link.id)) {
        continue;
      }

      const reverseKey = `${link.id}:${note.id}`;
      const isMutual = edgePairs.has(reverseKey);

      const curvature = !isMutual ? 0.25 : note.id < link.id ? 0.4 : -0.4;

      edges.push({
        id: `${note.id}-${link.id}`,
        source: note.id,
        target: link.id,
        type: "note",
        data: {
          color: nodeColorMap.get(note.id)!,
          curvature,
        },
      });
    }
  }

  return {
    nodes,
    edges,
  };
}
