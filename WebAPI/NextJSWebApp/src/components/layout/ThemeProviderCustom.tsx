import { ReactNode, useCallback, useMemo, useReducer, useState } from "react";
import { ThemeProvider, createTheme, CssBaseline } from "@mui/material";
import { createContext } from "react";

export interface ThemeProviderCustomProps {
  children: ReactNode;
}
type ThemeMode = "light" | "dark";

type ThemeAction = { type: "TOGGLE" };

function themeReducer(state: ThemeMode, action: ThemeAction): ThemeMode {
  switch (action.type) {
    case "TOGGLE":
      return state === "light" ? "dark" : "light";
    default:
      return state;
  }
}
export default function ThemeProviderCustom({
  children,
}: ThemeProviderCustomProps) {
  const [mode, dispatch] = useReducer(themeReducer, "light");

  const toggleTheme = useCallback(() => {
    dispatch({ type: "TOGGLE" });
  }, []);

  const theme = useMemo(
    () =>
      createTheme({
        palette: {
          mode,
        },
      }),
    [mode]
  );

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      {/* Expose toggler globally */}
      <ThemeContext.Provider value={{ mode, toggleTheme }}>
        {children}
      </ThemeContext.Provider>
    </ThemeProvider>
  );
}

export const ThemeContext = createContext({
  mode: "light",
  toggleTheme: () => {},
});
