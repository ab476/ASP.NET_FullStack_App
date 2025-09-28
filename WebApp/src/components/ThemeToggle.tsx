"use client";

import { useTheme } from "next-themes";
import { useEffect, useState } from "react";
import { IconButton } from "@mui/material";
import LightModeIcon from '@mui/icons-material/LightMode';
import DarkModeIcon from "@mui/icons-material/DarkMode";

export default function ThemeToggle() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => setMounted(true), []);
  if (!mounted) return null; // prevent hydration mismatch

  return (
    <IconButton onClick={() => setTheme(theme === "light" ? "dark" : "light")}>
      {theme === "light" ? <DarkModeIcon /> : <LightModeIcon />}
    </IconButton>
  );
}
