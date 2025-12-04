
import { Box, Typography } from "@mui/material";

export interface FooterProps {
  text?: string; // Optional override
}

export default function Footer({ text }: FooterProps) {
  return (
    <Box
      sx={{
        py: 2,
        textAlign: "center",
        borderTop: "1px solid",
        borderColor: "divider",
      }}
    >
      <Typography variant="body2" color="text.secondary">
        {text ?? `© ${new Date().getFullYear()} My App. All rights reserved.`}
      </Typography>
    </Box>
  );
}
