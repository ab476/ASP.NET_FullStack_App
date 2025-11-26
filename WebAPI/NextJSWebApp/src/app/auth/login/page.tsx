"use client";

import { useEffect, useState } from "react";
import {
  Box,
  Typography,
  TextField,
  Button,
  FormControlLabel,
  Checkbox,
  Alert,
} from "@mui/material";
import Link from "next/link";
import QuickSignIn from "@/components/auth/login/QuickSignIn";
import { tokenStore } from "@/lib/auth/TokenStore";
import { authEvents } from "@/lib/auth/AuthEvents";

export default function LoginPage() {
  const [form, setForm] = useState({
    email: "",
    password: "",
    rememberMe: false,
  });
  const [message, setMessage] = useState<string | null>(null);
  const [errors, setErrors] = useState<string[]>([]);
  const [token, setToken] = useState<string | null>(tokenStore.getToken());

  useEffect(() => {
    authEvents.subscribe((event) => {
      debugger;
      if (event.type === "TOKEN_UPDATED") {
        setToken(event.token);
      }
    });
  }, []);
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    tokenStore.setToken(form.email + "-token", { broadcast: true });
    // placeholder for API call
    // setMessage("Login successful!");
    // setErrors([]);
  };

  return (
    <Box
      sx={{ mt: 3, mb: 3, display: "flex", justifyContent: "center", px: 2 }}
    >
      <Box
        sx={{
          display: "flex",
          flexDirection: { xs: "column", md: "row" },
          boxShadow: 3,
          borderRadius: 4,
          maxWidth: 800,
          width: "100%",
          overflow: "hidden",
          backgroundColor: "#fff",
        }}
      >
        {/* Left: Login Form */}
        <Box
          sx={{
            flex: 1,
            p: 4,
            backgroundColor: "#f8f9fa",
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
          }}
        >
          <Typography
            variant="h5"
            fontWeight="bold"
            color="primary.main"
            align="center"
            mb={1}
          >
            Sign in to{" "}
            <Box component="span" color="text.primary">
              Dot Net
            </Box>
          </Typography>
          <Typography
            variant="body2"
            color="text.secondary"
            align="center"
            mb={3}
          >
            <Box component="span" fontWeight="bold">
              Welcome back!
            </Box>{" "}
            Please enter your credentials below to access your account.
          </Typography>

          {message && (
            <Alert severity="info" sx={{ mb: 2 }}>
              {message}
            </Alert>
          )}
          {errors.length > 0 && (
            <Alert severity="error" sx={{ mb: 2 }}>
              <strong>Please fix the errors below:</strong>
              <ul>
                {errors.map((err, i) => (
                  <li key={i}>{err}</li>
                ))}
              </ul>
            </Alert>
          )}

          <form onSubmit={handleSubmit} noValidate>
            <TextField
              label="Email"
              name="email"
              value={form.email}
              onChange={handleChange}
              fullWidth
              margin="normal"
              required
            />
            <TextField
              label="Password"
              type="password"
              name="password"
              value={form.password}
              onChange={handleChange}
              fullWidth
              margin="normal"
              required
            />
            <FormControlLabel
              control={
                <Checkbox
                  checked={form.rememberMe}
                  onChange={handleChange}
                  name="rememberMe"
                />
              }
              label="Remember Me"
            />
            <Button
              type="submit"
              variant="contained"
              color="primary"
              fullWidth
              sx={{ py: 1.5, mt: 2, mb: 2 }}
            >
              Sign In
            </Button>

            <Typography variant="body2" align="center" mb={1}>
              New to{" "}
              <Box component="span" fontWeight="bold">
                Dot Net Tutorials
              </Box>
              ?
              <Link href="/auth/register" passHref>
                <Box
                  color="primary.main"
                  sx={{ ml: 0.5, fontWeight: "bold", textDecoration: "none" }}
                  component="span"
                >
                  Create an account
                </Box>
              </Link>
            </Typography>
            <Typography variant="body2" align="center">
              <Link href="/auth/resend-email-confirmation" passHref>
                <Box
                  color="info.main"
                  sx={{ fontSize: "0.875rem", textDecoration: "none" }}
                  component="span"
                >
                  Resend email confirmation
                </Box>
              </Link>
            </Typography>
          </form>
        </Box>

        {/* Right: Social Login */}
        <Box
          sx={{
            flex: 1,
            p: 4,
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            backgroundColor: "#fff",
          }}
        >
          <QuickSignIn />
          Token: {token}
        </Box>
      </Box>
    </Box>
  );
}
