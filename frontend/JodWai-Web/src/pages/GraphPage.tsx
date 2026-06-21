import { useCallback, useState } from "react";
import {
  applyEdgeChanges,
  applyNodeChanges,
  type Node,
  type Edge,
  type NodeChange,
  type EdgeChange,
  Background,
  Controls,
  ReactFlow,
} from "@xyflow/react";
import "@xyflow/react/dist/style.css";

const initialNodes: Node[] = [
    {
      id: "n1",
      data: { label: "Node 1" },
      position: { x: 0, y: 0 },
      type: "input",
    },
    {
      id: "n2",
      data: { label: "Node 2" },
      position: { x: 100, y: 100 },
    },
  ];
  const initialEdges: Edge[] = [
    {
      id: "n1-n2",
      source: "n1",
      target: "n2",
      label: "connects with",
      type: "step",
    },
  ];
  

export default function GraphPage() {
  const [nodes, setNodes] = useState<Node[]>(initialNodes);
  const [edges, setEdges] = useState<Edge[]>(initialEdges);

  const onNodesChange = useCallback(
    (changes: NodeChange[]) =>
      setNodes((nodesSnapshot) => applyNodeChanges(changes, nodesSnapshot)),
    [],
  );
  const onEdgesChange = useCallback(
    (changes: EdgeChange[]) =>
      setEdges((edgesSnapshot) =>
        applyEdgeChanges<Edge>(changes, edgesSnapshot),
      ),
    [],
  );
  return (
    <div>
      <div>
        <h1 className="text-2xl font-bold">Graph Page</h1>
      </div>
      <div style={{ width: "80vw", height: "80vh" }}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          fitView
        >
          <Background />
          <Controls />
        </ReactFlow>
      </div>
    </div>
  );
}
