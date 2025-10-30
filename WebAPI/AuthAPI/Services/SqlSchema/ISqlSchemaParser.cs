namespace AuthAPI.Services.SqlSchema;

public interface ISqlSchemaParser
{
    public List<SqlTable> Parse(string sqlScript);
}
public record SqlColumn(string Name, string Type, bool Nullable);
public record SqlForeignKey(string Column, string ReferencedTable, string ReferencedColumn);
public record SqlIndex(string IndexName, string Table, string Column, bool IsUnique);
public record SqlTable(string Table, List<SqlColumn> Columns, List<string> PrimaryKeys, List<SqlForeignKey> ForeignKeys, List<SqlIndex> Indexes);