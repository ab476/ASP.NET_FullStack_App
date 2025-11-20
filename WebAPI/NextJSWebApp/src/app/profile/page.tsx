"use client";

import { Box, Typography, Table, TableBody, TableRow, TableCell, Button, Chip, Alert } from "@mui/material";

type ProfileViewModel = {
  email: string;
  userName: string;
  firstName: string;
  lastName?: string;
  dateOfBirth?: string; // ISO string
  phoneNumber?: string;
  lastLoggedIn?: string; // ISO string
  createdOn?: string; // ISO string
};

export default function ProfilePage({ profile = {
    email: "",
    userName: "",
    firstName: ""
} }: { profile: ProfileViewModel }) {
  const formatDate = (date?: string, showTime = false) => {
    if (!date) return "-";
    const d = new Date(date);
    return showTime
      ? d.toLocaleString(undefined, { day: "2-digit", month: "short", year: "numeric", hour: "2-digit", minute: "2-digit" })
      : d.toLocaleDateString(undefined, { day: "2-digit", month: "short", year: "numeric" });
  };

  return (
    <Box sx={{ minHeight: "68vh", maxWidth: 900, mx: "auto", py: 1 }}>
      {/* Welcome Message */}
      <Alert severity="info" sx={{ mb: 4, fontSize: "1.1rem", textAlign: "center" }}>
        <strong>Welcome to your profile, <Box component="span" color="primary.main">{profile.firstName}</Box>!</strong><br />
        Here you can review your account details and keep your information up-to-date.
      </Alert>

      {/* Profile Card */}
      <Box
        sx={{
          boxShadow: 3,
          borderRadius: 4,
          background: "linear-gradient(110deg, #f8fafd 70%, #e8f0fe 100%)",
        }}
      >
        {/* Header */}
        <Box sx={{ backgroundColor: "white", borderRadius: "16px 16px 0 0", textAlign: "center", py: 2 }}>
          <Typography variant="h5" fontWeight="bold" color="primary.main" mb={1}>
            Account Profile
          </Typography>
          <Chip label={profile.email} color="primary" sx={{ fontSize: "1rem", px: 3, py: 1 }} />
        </Box>

        {/* Body */}
        <Box sx={{ px: { xs: 2, sm: 5 }, py: 3 }}>
          <Table>
            <TableBody>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>First Name:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{profile.firstName}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>Last Name:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{profile.lastName || "-"}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>Phone:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{profile.phoneNumber || "-"}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>Date of Birth:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{formatDate(profile.dateOfBirth)}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>Account Created:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{formatDate(profile.createdOn, true)}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell sx={{ fontWeight: "bold", color: "text.secondary" }}>Last Logged In:</TableCell>
                <TableCell sx={{ fontWeight: "medium" }}>{formatDate(profile.lastLoggedIn, true)}</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </Box>

        {/* Footer */}
        <Box sx={{ backgroundColor: "white", borderRadius: "0 0 16px 16px", textAlign: "center", py: 3 }}>
          <Button
            variant="contained"
            color="info"
            sx={{ mr: 2, px: 5, py: 1.5, borderRadius: 50, fontWeight: "bold" }}
            href="/profile/edit"
          >
            Edit Profile
          </Button>
          <Button
            variant="contained"
            color="warning"
            sx={{ px: 5, py: 1.5, borderRadius: 50, fontWeight: "bold", color: "text.primary" }}
            href="/profile/change-password"
          >
            Change Password
          </Button>
        </Box>
      </Box>
    </Box>
  );
}
