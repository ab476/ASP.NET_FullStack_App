"use client";

import {
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  Box,
  Menu,
  MenuItem,
  Avatar,
  Badge,
  Tooltip,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import NotificationsIcon from "@mui/icons-material/Notifications";
import Brightness4Icon from "@mui/icons-material/Brightness4";
import { useContext, useState } from "react";
import { NavItem } from "./types";
import { ThemeContext } from "./ThemeProviderCustom";
import { useLayout } from "@/context/LayoutContext";

export interface HeaderProps {
  navItems: NavItem[];
  onMenuClick: () => void;
}

export default function Header({ navItems, onMenuClick }: HeaderProps) {
  const { toggleTheme } = useContext(ThemeContext);
  const { showAppBar } = useLayout();



  const [anchorElUser, setAnchorElUser] = useState<null | HTMLElement>(null);

  if (!showAppBar) return null;

  const openUserMenu = (e: React.MouseEvent<HTMLElement>) =>
    setAnchorElUser(e.currentTarget);

  const closeUserMenu = () => setAnchorElUser(null);

  return (
    <AppBar position="fixed" elevation={1}>
      <Toolbar>
        {/* Mobile drawer toggle */}
        <IconButton
          color="inherit"
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 2, display: { sm: "none" } }}
        >
          <MenuIcon />
        </IconButton>

        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          Enterprise App
        </Typography>

        {/* Desktop navigation ONLY when sidebar is NOT shown */}
        <Box
          sx={{
            display: {
              xs: "none",
              sm: showAppBar ? "flex" : "none",
            },
            gap: 3,
          }}
        >
          {navItems.map((item) => (
            <Typography key={item.label}>{item.label}</Typography>
          ))}
        </Box>

        <Tooltip title="Toggle Theme">
          <IconButton color="inherit" onClick={toggleTheme}>
            <Brightness4Icon />
          </IconButton>
        </Tooltip>

        <IconButton color="inherit">
          <Badge badgeContent={3} color="error">
            <NotificationsIcon />
          </Badge>
        </IconButton>

        <IconButton sx={{ ml: 2 }} onClick={openUserMenu}>
          <Avatar alt="User" src="/avatar.png" />
        </IconButton>

        <Menu anchorEl={anchorElUser} open={Boolean(anchorElUser)} onClose={closeUserMenu}>
          <MenuItem>Profile</MenuItem>
          <MenuItem>Settings</MenuItem>
          <MenuItem>Logout</MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
}
