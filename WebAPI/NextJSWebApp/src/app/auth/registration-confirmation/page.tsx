"use client"

import { Button, Box, Typography, Alert } from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import InfoIcon from "@mui/icons-material/Info";
import Link from "next/link";

export default function RegistrationConfirmationPage() {
  return (
    <Box
      sx={{
        minHeight: "65vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        p: 2,
      }}
    >
      <Box
        sx={{
          maxWidth: 550,
          width: "100%",
          p: 4,
          borderRadius: 4,
          boxShadow: 3,
          textAlign: "center",
        }}
      >
        <CheckCircleIcon sx={{ fontSize: 60, color: "success.main", mb: 2 }} />

        <Typography variant="h5" fontWeight="bold" color="success.main" mb={2}>
          Thank You for Registering!
        </Typography>

        <Typography sx={{ fontSize: "1.1rem", mb: 3 }}>
          <Box component="span" fontWeight="bold" color="success.main" display="block">
            {`We've sent a confirmation email to your inbox.`}
          </Box>
          <Box component="span" fontWeight="medium" color="text.primary" display="block">
            Please click the confirmation link in the email to activate your account.
          </Box>
        </Typography>

        <Alert
          icon={<InfoIcon />}
          severity="info"
          sx={{ display: "inline-flex", mx: "auto", mb: 4, fontSize: "1.05rem", py: 1, px: 2 }}
        >
          <Box component="span" fontWeight="bold" mr={1}>
            Can’t find the email?
          </Box>
          <Box component="span" color="text.secondary">
            Check your Spam or Junk folder.
          </Box>
        </Alert>

        <Link href="/auth/login" passHref>
          <Button variant="contained" color="primary" sx={{ px: 4, fontWeight: "bold" }}>
            Go to Login
          </Button>
        </Link>
      </Box>
    </Box>
  );
}
