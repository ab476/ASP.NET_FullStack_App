"use client";

import { useState } from "react";
import { TextField, Button, Box, Typography, CircularProgress } from "@mui/material";

export default function RegisterForm() {
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    dateOfBirth: "",
    phoneNumber: "",
    password: "",
    confirmPassword: "",
  });

  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const res = await fetch("http://localhost:5000/api/account/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form),
      });

      if (res.ok) {
        const data = await res.json();
        setMessage(data.message ?? "Registration successful!");
      } else {
        const error = await res.json();
        setMessage(
          Array.isArray(error) ? error.join(", ") : error.message || "Error occurred"
        );
      }
    } catch (err) {
      console.error(err);
      setMessage("Unexpected error occurred.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{
        maxWidth: 400,
        mx: "auto",
        p: 3,
        border: "1px solid #ddd",
        borderRadius: 2,
        boxShadow: 2,
        display: "flex",
        flexDirection: "column",
        gap: 2,
      }}
    >
      <Typography variant="h5" fontWeight="bold" textAlign="center">
        Register
      </Typography>

      <TextField
        label="First Name"
        name="firstName"
        value={form.firstName}
        onChange={handleChange}
        required
        fullWidth
      />

      <TextField
        label="Last Name"
        name="lastName"
        value={form.lastName}
        onChange={handleChange}
        fullWidth
      />

      <TextField
        label="Email"
        type="email"
        name="email"
        value={form.email}
        onChange={handleChange}
        required
        fullWidth
      />

      <TextField
        label="Date of Birth"
        type="date"
        name="dateOfBirth"
        value={form.dateOfBirth}
        onChange={handleChange}
        InputLabelProps={{ shrink: true }}
        fullWidth
      />

      <TextField
        label="Phone Number"
        type="tel"
        name="phoneNumber"
        value={form.phoneNumber}
        onChange={handleChange}
        required
        fullWidth
      />

      <TextField
        label="Password"
        type="password"
        name="password"
        value={form.password}
        onChange={handleChange}
        required
        fullWidth
      />

      <TextField
        label="Confirm Password"
        type="password"
        name="confirmPassword"
        value={form.confirmPassword}
        onChange={handleChange}
        required
        fullWidth
      />

      <Button
        type="submit"
        variant="contained"
        color="primary"
        fullWidth
        disabled={loading}
        sx={{ py: 1.5 }}
      >
        {loading ? <CircularProgress size={24} sx={{ color: "white" }} /> : "Register"}
      </Button>

      {message && (
        <Typography
          variant="body2"
          textAlign="center"
          sx={{ mt: 1, color: message.includes("success") ? "green" : "red" }}
        >
          {message}
        </Typography>
      )}
    </Box>
  );
}
