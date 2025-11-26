import { Button } from "@mui/material";
import Image from "next/image";

export interface SocialButtonProps {
  label: string;
  icon: string;
  color?: string;
  textColor?: string;
  variant?: "text" | "outlined" | "contained";
}

export default function SocialButton({
  label,
  icon,
  color,
  textColor = "#000",
  variant = "outlined",
}: SocialButtonProps) {
  return (
    <Button
      fullWidth
      variant={variant}
      sx={{
        backgroundColor: color,
        color: textColor,
        "&:hover": {
          opacity: 0.9,
          backgroundColor: color,
        },
      }}
      startIcon={
        <Image src={icon} alt={label} width={24} height={24} />
      }
    >
      {label}
    </Button>
  );
}
