import { useNavigate } from "react-router-dom";
import { useNoteGraphData } from "../hooks/useNoteGraphData";
import {
  Background,
  Controls,
  MiniMap,
  ReactFlow,
  useEdgesState,
  useNodesState,
  type NodeMouseHandler,
} from "@xyflow/react";
import type { GraphNode } from "../types";
import { useCallback, useEffect } from "react";
import NoteGraphNode from "./NoteGraphNode";
import NoteGraphEdge from "./NoteGraphEdge";
import "@xyflow/react/dist/style.css";

const nodeTypes = {
  note: NoteGraphNode,
};

const edgeTypes = {
  note: NoteGraphEdge,
};

export default function NoteGraphView() {
  // const navigate = useNavigate();

  const { graph, isLoading, isError } = useNoteGraphData();
  const [nodes, setNodes, onNodesChange] = useNodesState(graph.nodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(graph.edges);

  // sync เมื่อ graph เปลี่ยน (เช่น notes ถูกโหลดใหม่/แก้ไข)
  useEffect(() => {
    setNodes(graph.nodes);
    setEdges(graph.edges);
  }, [graph, setNodes, setEdges]);

  // const handleNodeClick = useCallback<NodeMouseHandler<GraphNode>>(
  //   (_, node) => {
  //     navigate(`/notes/${node.id}`);
  //   },
  //   [navigate],
  // );

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        Loading graph...
      </div>
    );
  }

  if (isError) {
    return (
      <div className="flex h-full items-center justify-center">
        Failed to load notes. Please try again.
      </div>
    );
  }

  if (graph.nodes.length === 0) {
    return (
      <div className="flex h-full flex-col items-center justify-center gap-2">
        <h2 className="text-lg font-semibold">No notes yet</h2>
        <p className="text-gray-500">
          Create your first note to start building your graph.
        </p>
      </div>
    );
  }

  return (
    <ReactFlow
      nodes={nodes}
      edges={edges}
      onNodesChange={onNodesChange}
      onEdgesChange={onEdgesChange}
      nodeTypes={nodeTypes}
      edgeTypes={edgeTypes}
      // onNodeClick={handleNodeClick}
      fitView
    >
      <Background />
      <MiniMap
        nodeColor={(node) => {
          const color = (node.data as GraphNode["data"] | undefined)?.color;
          return typeof color === "string" ? color : "#888";
        }}
        nodeStrokeWidth={2}
        pannable
        zoomable
      />
      <Controls />
    </ReactFlow>
  );
}
