import React from "react";
import {
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import { SqlTable } from "./types";
import SqlColumnTable from "./SqlColumnTable";
import SqlPrimaryKeys from "./SqlPrimaryKeys";
import SqlForeignKeysTable from "./SqlForeignKeysTable";
import SqlIndexesTable from "./SqlIndexesTable";

interface SqlTableCardProps {
  table: SqlTable;
}

const SqlTableCard: React.FC<SqlTableCardProps> = ({ table }) => (
  <Accordion>
    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
      <Typography variant="h6">{table.table}</Typography>
    </AccordionSummary>

    <AccordionDetails>
      <SqlColumnTable
        columns={table.columns}
        primaryKeys={table.primaryKeys}
        foreignKeys={table.foreignKeys}
        indexes={table.indexes}
      />
      <SqlPrimaryKeys primaryKeys={table.primaryKeys} />
      <SqlForeignKeysTable foreignKeys={table.foreignKeys} />
      <SqlIndexesTable indexes={table.indexes} />
    </AccordionDetails>
  </Accordion>
);

export default SqlTableCard;
