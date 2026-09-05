import { Navigate } from "react-router-dom";

/** Legacy hub → LIF-SCR-001. */
export function LifOffboardingPage() {
  return <Navigate to="/lif" replace />;
}
