import { Box } from "@mui/material";
import SocialButton, { SocialButtonProps } from "./SocialButton";

const providers: SocialButtonProps[] = [
  {
    label: "Google",
    icon: "/logos/google-logo.png",
    variant: "outlined",
    color: undefined,
    textColor: undefined,
  },
  {
    label: "Facebook",
    icon: "/logos/facebook-new.png",
    color: "#3b5998",
    textColor: "#fff",
  },
  {
    label: "GitHub",
    icon: "/logos/github.png",
    variant: "outlined",
    color: undefined,
    textColor: undefined,
  },
  {
    label: "Microsoft",
    icon: "/logos/microsoft.png",
    color: "#0078d4",
    textColor: "#fff",
  },
];

export default function SocialButtonList() {
  return (
    <Box sx={{ display: "grid", gap: 2, mb: 2 }}>
      {providers.map((p) => (
        <SocialButton key={p.label} {...p} />
      ))}
    </Box>
  );
}
