"use client";

import { useState } from "react";
import { Box, Typography, TextField, Button, FormControlLabel, Checkbox, Divider, Alert } from "@mui/material";
import Link from "next/link";

export default function LoginPage() {
  const [form, setForm] = useState({ email: "", password: "", rememberMe: false });
  const [message, setMessage] = useState<string | null>(null);
  const [errors, setErrors] = useState<string[]>([]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // placeholder for API call
    // setMessage("Login successful!");
    // setErrors([]);
  };

  return (
    <Box sx={{ mt: 3, mb: 3, display: "flex", justifyContent: "center", px: 2 }}>
      <Box sx={{ display: "flex", flexDirection: { xs: "column", md: "row" }, boxShadow: 3, borderRadius: 4, maxWidth: 800, width: "100%", overflow: "hidden", backgroundColor: "#fff" }}>
        
        {/* Left: Login Form */}
        <Box sx={{ flex: 1, p: 4, backgroundColor: "#f8f9fa", display: "flex", flexDirection: "column", justifyContent: "center" }}>
          <Typography variant="h5" fontWeight="bold" color="primary.main" align="center" mb={1}>
            Sign in to <Box component="span" color="text.primary">Dot Net Tutorials</Box>
          </Typography>
          <Typography variant="body2" color="text.secondary" align="center" mb={3}>
            <Box component="span" fontWeight="bold">Welcome back!</Box> Please enter your credentials below to access your account.
          </Typography>

          {message && <Alert severity="info" sx={{ mb: 2 }}>{message}</Alert>}
          {errors.length > 0 && (
            <Alert severity="error" sx={{ mb: 2 }}>
              <strong>Please fix the errors below:</strong>
              <ul>
                {errors.map((err, i) => (<li key={i}>{err}</li>))}
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
              control={<Checkbox checked={form.rememberMe} onChange={handleChange} name="rememberMe" />}
              label="Remember Me"
            />
            <Button type="submit" variant="contained" color="primary" fullWidth sx={{ py: 1.5, mt: 2, mb: 2 }}>
              Sign In
            </Button>

            <Typography variant="body2" align="center" mb={1}>
              New to <Box component="span" fontWeight="bold">Dot Net Tutorials</Box>?
              <Link href="/auth/register" passHref>
                <Box component="a" color="primary.main" sx={{ ml: 0.5, fontWeight: "bold", textDecoration: "none" }}>Create an account</Box>
              </Link>
            </Typography>
            <Typography variant="body2" align="center">
              <Link href="/auth/resend-email-confirmation" passHref>
                <Box component="a" color="info.main" sx={{ fontSize: "0.875rem", textDecoration: "none" }}>Resend email confirmation</Box>
              </Link>
            </Typography>
          </form>
        </Box>

        {/* Right: Social Login */}
        <Box sx={{ flex: 1, p: 4, display: "flex", flexDirection: "column", justifyContent: "center", alignItems: "center", backgroundColor: "#fff" }}>
          <Box sx={{ width: "100%", maxWidth: 280 }}>
            <Typography variant="h6" fontWeight="bold" color="primary.main" align="center" mb={1}>Quick sign-in</Typography>
            <Typography variant="body2" color="text.secondary" align="center" mb={2}>Use your social account for instant access:</Typography>

            <Box sx={{ display: "grid", gap: 2, mb: 2 }}>
              <Button variant="outlined" color="primary" fullWidth startIcon={<img src="https://img.icons8.com/color/32/000000/google-logo.png" alt="Google" style={{ width: 24 }} />}>
                Google
              </Button>
              <Button fullWidth sx={{ backgroundColor: "#3b5998", color: "#fff" }} startIcon={<img src="https://img.icons8.com/fluency/32/000000/facebook-new.png" alt="Facebook" style={{ width: 24 }} />}>
                Facebook
              </Button>
              <Button variant="outlined" color="inherit" fullWidth startIcon={<img src="https://img.icons8.com/ios-filled/32/000000/github.png" alt="GitHub" style={{ width: 24 }} />}>
                GitHub
              </Button>
              <Button fullWidth sx={{ backgroundColor: "#0078d4", color: "#fff" }} startIcon={<img src="https://img.icons8.com/color/32/000000/microsoft.png" alt="Microsoft" style={{ width: 24 }} />}>
                Microsoft
              </Button>
            </Box>

            <Typography variant="caption" color="text.secondary" align="center" mb={1}>
              We never post anything without your permission.
            </Typography>
            <Divider sx={{ mb: 1 }} />
            <Typography variant="caption" color="text.secondary" align="center">
              Need help? <Link href="mailto:support@dotnettutorials.net"><Box component="a" fontWeight="bold" sx={{ textDecoration: "none" }}>Contact Support</Box></Link>
            </Typography>
          </Box>
        </Box>

      </Box>
    </Box>
  );
}
