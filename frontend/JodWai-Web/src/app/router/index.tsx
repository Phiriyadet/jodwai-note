import { createBrowserRouter } from "react-router-dom";
import MainLayout from "../../components/layout/MainLayout";
import NotesPage from "../../pages/NotePage";
import AboutPage from "../../pages/AboutPage";
import GraphPage from "../../pages/GraphPage";

export const router = createBrowserRouter([
  {
    element: <MainLayout />,
    children: [
      {
        path: "/",
        element: <NotesPage />,
      },
      {
        path: "/graph",
        element: <GraphPage />,
      },
      {
        path: "/about",
        element: <AboutPage />,
      },
    ],
  },
]);
