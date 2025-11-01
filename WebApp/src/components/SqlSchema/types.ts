export interface SqlColumn {
  name: string;
  type: string;
  nullable: boolean;
}

export interface SqlForeignKey {
  column: string;
  referencedTable: string;
  referencedColumn: string;
}

export interface SqlIndex {
  indexName: string;
  table: string;
  column: string;
  isUnique: boolean;
}

export interface SqlTable {
  table: string;
  columns: SqlColumn[];
  primaryKeys: string[];
  foreignKeys: SqlForeignKey[];
  indexes: SqlIndex[];
}
