import SqlTableList, { SqlTable } from "@/components/SqlSchema/SqlTableList";
import React from "react";

const mockTables: SqlTable[] = [
  {
    table: "Users",
    columns: [
      { name: "Id", type: "int", nullable: false },
      { name: "Name", type: "nvarchar(100)", nullable: false },
      { name: "Email", type: "nvarchar(255)", nullable: true }
    ],
    primaryKeys: ["Id"],
    foreignKeys: [],
    indexes: [{ indexName: "IX_Users_Email", table: "Users", column: "Email", isUnique: true }]
  },
  {
    table: "Orders",
    columns: [
      { name: "OrderId", type: "int", nullable: false },
      { name: "UserId", type: "int", nullable: false },
      { name: "Total", type: "decimal(10,2)", nullable: false }
    ],
    primaryKeys: ["OrderId"],
    foreignKeys: [{ column: "UserId", referencedTable: "Users", referencedColumn: "Id" }],
    indexes: []
  }
];

export default function SqlSchemaPage() {
  return <SqlTableList tables={mockTables} />;
}
