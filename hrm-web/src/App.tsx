import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AppLayout, RequireAuth } from "./layout/AppLayout";
import { EmployeeFormPage } from "./pages/EmployeeFormPage";
import { EmployeeListPage } from "./pages/EmployeeListPage";
import { IamAccountDetailPage } from "./pages/IamAccountDetailPage";
import { IamAccountListPage } from "./pages/IamAccountListPage";
import { MyProfilePage } from "./pages/MyProfilePage";
import { LineManagerQueuePage } from "./pages/LineManagerQueuePage";
import { LeaveC1QueuePage } from "./pages/LeaveC1QueuePage";
import { LeaveC2QueuePage } from "./pages/LeaveC2QueuePage";
import { LeavePage } from "./pages/LeavePage";
import { LoginPage } from "./pages/LoginPage";
import { PayAllowancePage } from "./pages/PayAllowancePage";
import { PayPayslipPage } from "./pages/PayPayslipPage";
import { PayPeriodPage } from "./pages/PayPeriodPage";
import { TimImportPage } from "./pages/TimImportPage";
import { TimPeriodPage } from "./pages/TimPeriodPage";
import { TimTemplatePage } from "./pages/TimTemplatePage";
import { isHr, isHrOrIt, isIt, useCurrentUser } from "./hooks/useCurrentUser";
import { PrbCasesPage } from "./pages/PrbCasesPage";
import { PrbMyMilestonesPage } from "./pages/PrbMyMilestonesPage";
import { LifOffboardingPage } from "./pages/LifOffboardingPage";
import { LifOnboardingPage } from "./pages/LifOnboardingPage";
import type { ReactNode } from "react";

function RequirePayHr({ children }: { children: ReactNode }) {
  const user = useCurrentUser();
  if (!isHr(user)) return <Navigate to="/pay/payslips" replace />;
  return <>{children}</>;
}

function RequireHr({ children }: { children: ReactNode }) {
  const user = useCurrentUser();
  if (!isHr(user)) return <Navigate to="/profile" replace />;
  return <>{children}</>;
}

function RequireTimTemplate({ children }: { children: ReactNode }) {
  const user = useCurrentUser();
  if (!isHrOrIt(user)) return <Navigate to="/profile" replace />;
  return <>{children}</>;
}

function RequirePrbHr({ children }: { children: ReactNode }) {
  const user = useCurrentUser();
  const ok = user.roles.some((r) => r === "IAM-ROLE-HR" || r === "IAM-ROLE-PGD");
  if (!ok) return <Navigate to="/prb/me" replace />;
  return <>{children}</>;
}

/** LIF on/off — HR/PGD + IT (provision / khóa Git·CRM). */
function RequireLifActor({ children }: { children: ReactNode }) {
  const user = useCurrentUser();
  const ok =
    isHr(user) ||
    isIt(user) ||
    user.roles.some((r) => r === "IAM-ROLE-PGD");
  if (!ok) return <Navigate to="/profile" replace />;
  return <>{children}</>;
}

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
          <Route path="/" element={<Navigate to="/profile" replace />} />
          <Route path="/profile" element={<MyProfilePage />} />
          <Route path="/leave" element={<LeavePage channel="web" />} />
          <Route path="/leave/m" element={<LeavePage channel="mobile" />} />
          <Route path="/pay/payslips" element={<PayPayslipPage channel="web" />} />
          <Route path="/pay/m/payslips" element={<PayPayslipPage channel="mobile" />} />
          <Route path="/leave/c1" element={<LeaveC1QueuePage />} />
          <Route path="/leave/c2" element={<LeaveC2QueuePage />} />
          <Route
            path="/tim/templates"
            element={
              <RequireTimTemplate>
                <TimTemplatePage />
              </RequireTimTemplate>
            }
          />
          <Route
            path="/tim/imports"
            element={
              <RequireHr>
                <TimImportPage />
              </RequireHr>
            }
          />
          <Route
            path="/tim/periods"
            element={
              <RequireHr>
                <TimPeriodPage />
              </RequireHr>
            }
          />
          <Route
            path="/pay/periods"
            element={
              <RequirePayHr>
                <PayPeriodPage />
              </RequirePayHr>
            }
          />
          <Route
            path="/pay/allowances"
            element={
              <RequirePayHr>
                <PayAllowancePage />
              </RequirePayHr>
            }
          />
          <Route path="/prb/me" element={<PrbMyMilestonesPage />} />
          <Route
            path="/prb/cases"
            element={
              <RequirePrbHr>
                <PrbCasesPage />
              </RequirePrbHr>
            }
          />
          <Route
            path="/lif/onboarding"
            element={
              <RequireLifActor>
                <LifOnboardingPage />
              </RequireLifActor>
            }
          />
          <Route
            path="/lif/offboarding"
            element={
              <RequireLifActor>
                <LifOffboardingPage />
              </RequireLifActor>
            }
          />
          <Route path="/employees" element={<EmployeeListPage />} />
          <Route path="/employees/new" element={<EmployeeFormPage />} />
          <Route path="/employees/:id" element={<EmployeeFormPage />} />
          <Route path="/line-manager-changes" element={<LineManagerQueuePage />} />
          <Route path="/iam/accounts" element={<IamAccountListPage />} />
          <Route path="/iam/accounts/:id" element={<IamAccountDetailPage />} />
        </Route>
        <Route path="*" element={<Navigate to="/profile" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
