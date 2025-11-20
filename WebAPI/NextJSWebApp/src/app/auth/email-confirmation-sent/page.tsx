import { Box, Typography, Button, Alert, Stack } from "@mui/material";
import EmailOutlinedIcon from "@mui/icons-material/EmailOutlined";

export default function EmailConfirmationSentPage() {
  return (
    <Box
      sx={{
        minHeight: "65vh",
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        px: 2,
      }}
    >
      <Box
        sx={{
          width: "100%",
          maxWidth: 600,
          borderRadius: 4,
          boxShadow: 3,
          p: 4,
        }}
      >
        <Stack spacing={2} alignItems="center">
          <EmailOutlinedIcon color="primary" sx={{ fontSize: 40 }} />
          <Typography variant="h5" fontWeight="bold" color="primary" textAlign="center">
            Email Confirmation Sent
          </Typography>

          <Alert severity="info" sx={{ textAlign: "center", fontSize: "1.06rem" }}>
            If the email address you entered is registered, a confirmation link has been sent.<br />
            <strong>Please check your inbox and also your Spam or Junk folder.</strong>
          </Alert>

          <Button
            href="/auth/login"
            variant="contained"
            color="primary"
            sx={{ mt: 2, px: 4, py: 1.5, fontWeight: "bold" }}
          >
            Back to Login
          </Button>
        </Stack>
      </Box>
    </Box>
  );
}
