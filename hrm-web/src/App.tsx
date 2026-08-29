import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AppLayout, RequireAuth } from "./layout/AppLayout";
import { EmployeeFormPage } from "./pages/EmployeeFormPage";
import { EmployeeListPage } from "./pages/EmployeeListPage";
import { LineManagerQueuePage } from "./pages/LineManagerQueuePage";
import { LoginPage } from "./pages/LoginPage";

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route
          element={
            <RequireAuth>
              <AppLayout />
            </RequireAuth>
          }
        >
          <Route path="/" element={<Navigate to="/employees" replace />} />
          <Route path="/employees" element={<EmployeeListPage />} />
          <Route path="/employees/new" element={<EmployeeFormPage />} />
          <Route path="/employees/:id" element={<EmployeeFormPage />} />
          <Route path="/line-manager-changes" element={<LineManagerQueuePage />} />
        </Route>
        <Route path="*" element={<Navigate to="/employees" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
