import React from "react";
import {
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
} from "@mui/material";
import { SqlIndex } from "./types";

interface SqlIndexesTableProps {
  indexes: SqlIndex[];
}

const SqlIndexesTable: React.FC<SqlIndexesTableProps> = ({ indexes }) =>
  indexes.length > 0 ? (
    <>
      <Typography variant="subtitle1" sx={{ mt: 2 }}>
        Indexes
      </Typography>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Column</TableCell>
            <TableCell>Unique</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {indexes.map((idx, i) => (
            <TableRow key={i}>
              <TableCell>{idx.indexName}</TableCell>
              <TableCell>{idx.column}</TableCell>
              <TableCell>{idx.isUnique ? "Yes" : "No"}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </>
  ) : null;

export default SqlIndexesTable;
