import { Navigate } from "react-router-dom";

/** Legacy hub → TIM-SCR-001. */
export function TimPeriodPage() {
  return <Navigate to="/tim" replace />;
}
