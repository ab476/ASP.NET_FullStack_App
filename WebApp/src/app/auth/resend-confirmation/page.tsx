"use client";

import { useState } from "react";
import { Box, Typography, TextField, Button, Alert, Stack } from "@mui/material";
import MailOutlineIcon from "@mui/icons-material/MailOutline";

export default function ResendConfirmationPage() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage(null);
    setError(null);

    if (!email) {
      setError("Please enter your registered email address.");
      setLoading(false);
      return;
    }

    try {
      const res = await fetch("/api/account/resend-email-confirmation", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });

      if (res.ok) {
        const data = await res.json();
        setMessage(data.message ?? "Confirmation email sent successfully!");
      } else {
        const err = await res.json();
        setError(err.message ?? "Error occurred while sending email.");
      }
    } catch (err) {
      console.error(err);
      setError("Unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "65vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        px: 2,
      }}
    >
      <Box
        sx={{
          width: "100%",
          maxWidth: 540,
          borderRadius: 4,
          boxShadow: 3,
          p: 4,
          background: "linear-gradient(120deg, #f3faff 70%, #e8f0fe 100%)",
        }}
      >
        <Stack spacing={2} alignItems="center">
          <MailOutlineIcon color="primary" sx={{ fontSize: 48 }} />
          <Typography variant="h5" fontWeight="bold" color="primary" textAlign="center">
            Resend Email Confirmation
          </Typography>
          <Alert severity="info" sx={{ textAlign: "center", fontSize: "1.05rem" }}>
            Please enter your <strong>registered email address</strong> below.<br />
            We’ll send you a new confirmation link right away.
          </Alert>

          {message && <Alert severity="success" sx={{ width: "100%" }}>{message}</Alert>}
          {error && <Alert severity="error" sx={{ width: "100%" }}>{error}</Alert>}

          <Box component="form" onSubmit={handleSubmit} sx={{ width: "100%" }}>
            <TextField
              label="Email"
              placeholder="Enter your registered email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              fullWidth
              required
              margin="normal"
            />
            <Button
              type="submit"
              variant="contained"
              color="primary"
              fullWidth
              sx={{ py: 1.5, mt: 1 }}
              disabled={loading}
            >
              {loading ? "Sending..." : "Send Confirmation Email"}
            </Button>
          </Box>

          <Typography variant="body2" color="text.secondary" textAlign="center">
            Remembered your password?{" "}
            <a href="/auth/login" style={{ color: "#1976d2", fontWeight: 600, textDecoration: "none" }}>
              Login here
            </a>
          </Typography>
        </Stack>
      </Box>
    </Box>
  );
}
