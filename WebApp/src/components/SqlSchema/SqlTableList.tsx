import React from "react";
import { Stack } from "@mui/material";
import { SqlTable } from "./types";
import SqlTableCard from "./SqlTableCard";

interface SqlTableListProps {
  tables: SqlTable[];
}

const SqlTableList: React.FC<SqlTableListProps> = ({ tables }) => (
  <Stack spacing={2}>
    {tables.map((t) => (
      <SqlTableCard key={t.table} table={t} />
    ))}
  </Stack>
);

export default SqlTableList;
