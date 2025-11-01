import React, { JSX } from "react";
import {
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Stack,
} from "@mui/material";
import KeyIcon from "@mui/icons-material/VpnKey";
import LinkIcon from "@mui/icons-material/Link";
import StarIcon from "@mui/icons-material/Star";
import TagIcon from "@mui/icons-material/Tag";
import { SqlColumn, SqlForeignKey, SqlIndex } from "./types";

interface SqlColumnTableProps {
  columns: SqlColumn[];
  primaryKeys: string[];
  foreignKeys: SqlForeignKey[];
  indexes: SqlIndex[];
}

const SqlColumnTable: React.FC<SqlColumnTableProps> = ({
  columns,
  primaryKeys,
  foreignKeys,
  indexes,
}) => {
  const isPrimary = (name: string) => primaryKeys.includes(name);
  const isForeign = (name: string) =>
    foreignKeys.some((fk) => fk.column === name);
  const isUnique = (name: string) =>
    indexes.some((i) => i.column === name && i.isUnique);
  const isIndexed = (name: string) =>
    indexes.some((i) => i.column === name && !i.isUnique);

  const getIcons = (name: string) => {
    const icons: JSX.Element[] = [];
    if (isPrimary(name))
      icons.push(<KeyIcon key="pk" fontSize="small" color="primary" />);
    if (isForeign(name))
      icons.push(<LinkIcon key="fk" fontSize="small" color="secondary" />);
    if (isUnique(name))
      icons.push(<StarIcon key="unique" fontSize="small" color="warning" />);
    if (isIndexed(name))
      icons.push(<TagIcon key="index" fontSize="small" color="action" />);
    return icons;
  };

  return (
    <>
      <Typography variant="subtitle1" gutterBottom>
        Columns
      </Typography>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Type</TableCell>
            <TableCell>Nullable</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {columns.map((col) => (
            <TableRow key={col.name}>
              <TableCell>
                <Stack direction="row" spacing={0.5} alignItems="center">
                  {getIcons(col.name)}
                  <Typography>{col.name}</Typography>
                </Stack>
              </TableCell>
              <TableCell>{col.type}</TableCell>
              <TableCell>{col.nullable ? "Yes" : "No"}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </>
  );
};

export default SqlColumnTable;
