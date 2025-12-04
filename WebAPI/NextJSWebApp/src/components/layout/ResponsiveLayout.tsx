"use client";

import { Box, Toolbar } from "@mui/material";
import { useState, ReactNode, useCallback } from "react";
import Header from "./Header";
import Sidebar from "./Sidebar";
import Footer from "./Footer";
import BreadcrumbsBar from "./BreadcrumbsBar";
import ThemeProviderCustom from "./ThemeProviderCustom";
import { NavItem } from "./types";
import { useLayout } from "@/context/LayoutContext";

export interface ResponsiveLayoutProps {
  children: ReactNode;
  navItems: NavItem[];
}

export default function ResponsiveLayout({
  children,
  navItems,
}: ResponsiveLayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { showSidebar, showAppBar } = useLayout();
  const toggleMenu = useCallback(() => setMobileOpen((o) => !o), []);

  return (
    <ThemeProviderCustom>
      <Box sx={{ display: "flex", minHeight: "100vh" }}>
        {/* Sidebar (conditionally rendered) */}
        {showSidebar && (
          <Sidebar
            navItems={navItems}
            mobileOpen={mobileOpen}
            onClose={toggleMenu}
          />
        )}

        <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
          {/* AppBar (conditionally rendered) */}
          {showAppBar && (
            <Header navItems={navItems} onMenuClick={toggleMenu} />
          )}

          {/* Toolbar offset only if AppBar exists */}
          {showAppBar && <Toolbar />}

          {/* Breadcrumb only if any UI is visible */}
          {(showAppBar || showSidebar) && <BreadcrumbsBar />}

          {children}

          <Footer />
        </Box>
      </Box>
    </ThemeProviderCustom>
  );
}
