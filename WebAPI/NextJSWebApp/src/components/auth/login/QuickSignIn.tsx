import { Box, Typography, Divider, Link } from "@mui/material";
import SocialButtonList from "./SocialButtonList";
const QuickSignIn = () => {
  return (
    <Box sx={{ width: "100%", maxWidth: 280 }}>
      <Typography
        variant="h6"
        fontWeight="bold"
        color="primary.main"
        align="center"
        mb={1}
      >
        Quick sign-in
      </Typography>
      <Typography variant="body2" color="text.secondary" align="center" mb={2}>
        Use your social account for instant access:
      </Typography>

      <SocialButtonList />

      <Typography
        variant="caption"
        color="text.secondary"
        align="center"
        mb={1}
      >
        We never post anything without your permission.
      </Typography>
      <Divider sx={{ mb: 1 }} />
      <Typography variant="caption" color="text.secondary" align="center">
        Need help?{" "}
        <Link href="mailto:support@dotnettutorials.net">
          <Box
            fontWeight="bold"
            sx={{ textDecoration: "none" }}
            component="span"
          >
            Contact Support
          </Box>
        </Link>
      </Typography>
    </Box>
  );
};

export default QuickSignIn;
