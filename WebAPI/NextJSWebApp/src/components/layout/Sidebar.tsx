"use client";

import {
  Drawer,
  Toolbar,
  Box,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  IconButton,
} from "@mui/material";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import MenuOpenIcon from "@mui/icons-material/MenuOpen";
import { NavItem } from "./types";
import { useState } from "react";
import { useLayout } from "@/context/LayoutContext";

const drawerWidth = 240;
export interface SidebarProps {
  mobileOpen: boolean;
  onClose: () => void;
  navItems: NavItem[];
}

export default function Sidebar({ mobileOpen, onClose, navItems }: SidebarProps) {
  const [collapsed, setCollapsed] = useState(false);
  const { showSidebar } = useLayout();

  if (!showSidebar) return null;

  const toggleCollapse = () => setCollapsed((v) => !v);

  const drawerContent = (
    <Box sx={{ width: collapsed ? 70 : drawerWidth }}>
      <Toolbar />
      <IconButton onClick={toggleCollapse} sx={{ ml: 1 }}>
        {collapsed ? <MenuOpenIcon /> : <ChevronLeftIcon />}
      </IconButton>

      <List>
        {navItems.map((item) => (
          <ListItemButton key={item.label}>
            <ListItemIcon>{item.icon}</ListItemIcon>
            {!collapsed && <ListItemText primary={item.label} />}
          </ListItemButton>
        ))}
      </List>
    </Box>
  );

  return (
    <>
      {/* Mobile Drawer */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onClose}
        sx={{ display: { xs: "block", sm: "none" } }}
      >
        {drawerContent}
      </Drawer>

      {/* Desktop Drawer */}
      <Drawer
        variant="permanent"
        open
        sx={{
          display: { xs: "none", sm: "block" },
          width: collapsed ? 70 : drawerWidth,
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: collapsed ? 70 : drawerWidth,
            transition: "width 0.3s",
          },
        }}
      >
        {drawerContent}
      </Drawer>
    </>
  );
}
