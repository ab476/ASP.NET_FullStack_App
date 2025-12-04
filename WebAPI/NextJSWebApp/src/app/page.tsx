import { Box, Typography, Paper } from "@mui/material";
import HomeIcon from "@mui/icons-material/Home";

export const metadata = {
  title: "Home",
  description: "Dashboard Home Page",
};

export default function HomePage() {
  return (
    <Box
      sx={{
        p: 3,
      }}
    >
      <Paper
        elevation={1}
        sx={{
          display: "flex",
          alignItems: "center",
          gap: 2,
          p: 3,
        }}
      >
        <HomeIcon fontSize="large" />

        <Box>
          <Typography variant="h5" fontWeight={600}>
            Home
          </Typography>

          <Typography variant="body2" color="text.secondary">
            This is your main dashboard home page.
          </Typography>
        </Box>
      </Paper>
    </Box>
  );
}
