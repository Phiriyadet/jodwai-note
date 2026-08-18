import { useMemo } from "react";
import { buildGraphElements } from "../utils/buildGraphElements";
import { useAllNotesQuery } from "../../note/queries/useNotesQuery";
import { layoutWithDagre } from "../utils/layoutWithDagre";

export function useNoteGraphData() {
  const { data: notes = [], isLoading, isError } = useAllNotesQuery();

  const graph = useMemo(() => {
    const elements = buildGraphElements(notes);
    console.log(
      "nodes:",
      elements.nodes.length,
      "edges:",
      elements.edges.length,
      elements.edges,
    );
    const layoutedNodes = layoutWithDagre(elements.nodes, elements.edges);
    return { nodes: layoutedNodes, edges: elements.edges };
  }, [notes]);

  return { graph, isLoading, isError };
}
