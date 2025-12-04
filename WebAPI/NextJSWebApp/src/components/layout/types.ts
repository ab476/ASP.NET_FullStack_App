import { ReactNode } from "react";

export interface NavItem {
  label: string;
  icon: ReactNode;
  href?: string;
  children?: NavItem[]; // support nested menus
}
