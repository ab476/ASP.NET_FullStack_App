"use client";

import { Breadcrumbs, Typography, Link, Box } from "@mui/material";
import { usePathname } from "next/navigation";

export default function BreadcrumbsBar() {
  const pathname = usePathname();
  const parts = pathname.split("/").filter(Boolean);

  return (
    <Box sx={{ mb: 2 }}>
      <Breadcrumbs>
        <Link underline="hover" href="/">
          Home
        </Link>

        {parts.map((part, i) => {
          const isLast = i === parts.length - 1;
          const href = "/" + parts.slice(0, i + 1).join("/");

          return isLast ? (
            <Typography key={href}>{part}</Typography>
          ) : (
            <Link underline="hover" key={href} href={href}>
              {part}
            </Link>
          );
        })}
      </Breadcrumbs>
    </Box>
  );
}
