import { useOutletContext } from "react-router-dom";
import type { CurrentUser } from "../api/types";

export function useCurrentUser(): CurrentUser {
  return useOutletContext<CurrentUser>();
}

export function isHr(user: CurrentUser): boolean {
  return user.roles.some((role) => role === "IAM-ROLE-HR");
}

export function isHrOrIt(user: CurrentUser): boolean {
  return user.roles.some(
    (role) => role === "IAM-ROLE-HR" || role === "IAM-ROLE-IT",
  );
}

export function isIt(user: CurrentUser): boolean {
  return user.roles.some((role) => role === "IAM-ROLE-IT");
}
