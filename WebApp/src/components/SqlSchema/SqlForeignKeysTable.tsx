import React from "react";
import {
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
} from "@mui/material";
import { SqlForeignKey } from "./types";

interface SqlForeignKeysTableProps {
  foreignKeys: SqlForeignKey[];
}

const SqlForeignKeysTable: React.FC<SqlForeignKeysTableProps> = ({
  foreignKeys,
}) =>
  foreignKeys.length > 0 ? (
    <>
      <Typography variant="subtitle1" sx={{ mt: 2 }}>
        Foreign Keys
      </Typography>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Column</TableCell>
            <TableCell>References</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {foreignKeys.map((fk, i) => (
            <TableRow key={i}>
              <TableCell>{fk.column}</TableCell>
              <TableCell>
                {fk.referencedTable}.{fk.referencedColumn}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </>
  ) : null;

export default SqlForeignKeysTable;
