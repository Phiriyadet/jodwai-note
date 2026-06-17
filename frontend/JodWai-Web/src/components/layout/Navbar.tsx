import { NavLink } from "react-router-dom";

export default function Navbar() {
  return (
    <nav className="border-b">
      <div>
        <div className="container mx-auto flex gap-4 p-4">
          <NavLink to="/">Notes</NavLink>

          <NavLink to="/graph">Graph</NavLink>

          <NavLink to="/about">About</NavLink>
        </div>
      </div>
    </nav>
  );
}
