import { Box, Typography, Button, Alert, Stack } from "@mui/material";
import HighlightOffIcon from "@mui/icons-material/HighlightOff";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";

export default function ErrorPage() {
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
          maxWidth: 540,
          borderRadius: 4,
          boxShadow: 3,
          p: 4,
          background: "linear-gradient(120deg, #fff3f4 70%, #ffeaea 100%)",
        }}
      >
        <Stack spacing={2} alignItems="center">
          <HighlightOffIcon color="error" sx={{ fontSize: 45 }} />
          <Typography variant="h5" fontWeight="bold" color="error" textAlign="center">
            Something Went Wrong
          </Typography>

          <Alert severity="error" sx={{ textAlign: "center", fontSize: "1.08rem" }}>
            We’re sorry, but an unexpected error has occurred.<br />
            Please try again later. If the problem persists, contact our support team.
          </Alert>

          <Button
            href="/"
            variant="outlined"
            color="error"
            startIcon={<ArrowBackIcon />}
            sx={{ mt: 2, px: 4, fontWeight: "bold" }}
          >
            Back to Home
          </Button>
        </Stack>
      </Box>
    </Box>
  );
}
