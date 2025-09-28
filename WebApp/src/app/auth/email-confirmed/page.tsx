"use client";

import { Box, Typography, Button, Alert } from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import Link from "next/link";

export default function EmailConfirmedPage() {
  return (
    <Box
      sx={{
        minHeight: "55vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        mt: 4,
        p: 2,
      }}
    >
      <Box
        sx={{
          maxWidth: 520,
          width: "100%",
          p: 4,
          borderRadius: 4,
          boxShadow: 3,
          textAlign: "center",
        }}
      >
        <CheckCircleIcon sx={{ fontSize: 60, color: "success.main", mb: 2 }} />

        <Typography variant="h5" fontWeight="bold" color="success.main" mb={3}>
          Email Verified Successfully
        </Typography>

        <Alert
          severity="success"
          sx={{ fontWeight: "bold", mb: 4, fontSize: "1.08rem" }}
        >
          Your email address has been verified and your account is now active.
        </Alert>

        <Typography sx={{ fontSize: "1.06rem", mb: 4 }}>
          You can now log in and explore all the features of{" "}
          <Box component="span" fontWeight="bold" color="primary.main">
            Dot Net Tutorials
          </Box>
          .
        </Typography>

        <Link href="/auth/login" passHref>
          <Button variant="contained" color="primary" sx={{ px: 4, fontWeight: "bold" }}>
            Login to Your Account
          </Button>
        </Link>
      </Box>
    </Box>
  );
}
