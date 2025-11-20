import React from "react";
import { Typography, Chip, Stack } from "@mui/material";

interface SqlPrimaryKeysProps {
  primaryKeys: string[];
}

const SqlPrimaryKeys: React.FC<SqlPrimaryKeysProps> = ({ primaryKeys }) =>
  primaryKeys.length > 0 ? (
    <>
      <Typography variant="subtitle1" sx={{ mt: 2 }}>
        Primary Keys
      </Typography>
      <Stack direction="row" spacing={1}>
        {primaryKeys.map((pk) => (
          <Chip key={pk} label={pk} color="primary" />
        ))}
      </Stack>
    </>
  ) : null;

export default SqlPrimaryKeys;
