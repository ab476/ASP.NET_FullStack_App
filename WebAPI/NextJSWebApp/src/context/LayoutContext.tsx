"use client";

import { createContext, useContext, useReducer, ReactNode, useMemo } from "react";

export type LayoutMode = "appbar" | "sidebar";

interface LayoutState {
  mode: LayoutMode;
}

type LayoutAction = {
  type: "SET_MODE";
  payload: LayoutMode;
};

const LayoutContext = createContext<{
  mode: LayoutMode;

  // auto-computed flags
  showSidebar: boolean;
  showAppBar: boolean;
  isAppBarMode: boolean;
  isSidebarMode: boolean;

  // actions
  switchToAppBar: () => void;
  switchToSidebar: () => void;

  dispatch: React.Dispatch<LayoutAction>;
} | null>(null);

function layoutReducer(state: LayoutState, action: LayoutAction): LayoutState {
  switch (action.type) {
    case "SET_MODE":
      return { ...state, mode: action.payload };
    default:
      return state;
  }
}
const defaultLayoutMode: LayoutMode = "sidebar"
export function LayoutProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(layoutReducer, {
    mode: defaultLayoutMode,
  });

  const value = useMemo(() => {
    const mode = state.mode;

    return {
      mode,

      // centralized computed logic
      showSidebar: mode === "sidebar",
      showAppBar: mode === "appbar",

      isAppBarMode: mode === "appbar",
      isSidebarMode: mode === "sidebar",

      // centralized actions
      switchToAppBar: () => dispatch({ type: "SET_MODE", payload: "appbar" }),
      switchToSidebar: () => dispatch({ type: "SET_MODE", payload: "sidebar" }),

      dispatch,
    };
  }, [state.mode]);

  return <LayoutContext.Provider value={value}>{children}</LayoutContext.Provider>;
}

export const useLayout = () => {
  const ctx = useContext(LayoutContext);
  if (!ctx) throw new Error("useLayout must be inside LayoutProvider");
  return ctx;
};
