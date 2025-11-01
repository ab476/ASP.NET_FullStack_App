using System.Text.RegularExpressions;

namespace AuthAPI.Services.SqlSchema;

public partial class SqlSchemaParserService : ISqlSchemaParser
{
    List<SqlTable> ISqlSchemaParser.Parse(string sqlScript)
    {
        var tables = new List<SqlTable>();

        var createTableRegex = TableRegex();

        foreach (Match match in createTableRegex.Matches(sqlScript))
        {
            var tableName = match.Groups[1].Value.Trim();
            var body = match.Groups[2].Value.Trim();

            var columns = new List<SqlColumn>();
            var primaryKeys = new List<string>();
            var foreignKeys = new List<SqlForeignKey>();

            var lines = body.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                {
                    ExtractConstraints(line, primaryKeys, foreignKeys);
                }
                else
                {
                    var column = ParseColumn(line);
                    if (column is not null)
                        columns.Add(column);
                }
            }

            tables.Add(new SqlTable(tableName, columns, primaryKeys, foreignKeys, new List<SqlIndex>()));
        }

        // Extract and attach indexes
        var indexes = ExtractIndexes(sqlScript);
        foreach (var table in tables)
        {
            table.Indexes.AddRange(indexes.Where(i => i.Table == table.Table));
        }

        return tables;
    }

    private static SqlColumn? ParseColumn(string line)
    {
        var match = ColumnRegex().Match(line);

        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value;
        var type = match.Groups["type"].Value;
        var isNullable = !match.Groups["null"].Value.Contains("NOT", StringComparison.OrdinalIgnoreCase);

        return new SqlColumn(name, type, isNullable);
    }

    private static void ExtractConstraints(string line, List<string> primaryKeys, List<SqlForeignKey> foreignKeys)
    {
        var pkMatch = PrimaryKeyRegex().Match(line);
        if (pkMatch.Success)
        {
            var pkColumns = SqlIdentifierRegex().Matches(pkMatch.Groups[1].Value)
                                 .Select(m => m.Groups[1].Value)
                                 .ToList();

            primaryKeys.AddRange(pkColumns);
        }

        var fkMatch = ForeginKeyRegex().Match(line);

        if (fkMatch.Success)
        {
            foreignKeys.Add(new SqlForeignKey(
                fkMatch.Groups[1].Value,
                fkMatch.Groups[2].Value,
                fkMatch.Groups[3].Value
            ));
        }
    }

    private static List<SqlIndex> ExtractIndexes(string sqlScript)
    {
        var indexRegex = IndexRegex();

        return [.. indexRegex.Matches(sqlScript)
            .Select(m => new SqlIndex(
                IndexName: m.Groups[2].Value,
                Table: m.Groups[3].Value,
                Column: m.Groups[4].Value,
                IsUnique: !string.IsNullOrEmpty(m.Groups[1].Value)
            ))];
    }

    [GeneratedRegex(@"CREATE TABLE\s+\[([^\]]+)\]\s*\(([^;]+?)\)\s*;", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
    private static partial Regex TableRegex();
    [GeneratedRegex(@"\[(?<name>[^\]]+)\]\s+(?<type>[^\s,]+)(?:.*?(?<null>NULL|NOT NULL))?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ColumnRegex();
    [GeneratedRegex(@"PRIMARY KEY\s*\((.*?)\)", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex PrimaryKeyRegex();
    [GeneratedRegex(@"\[(.*?)\]")]
    private static partial Regex SqlIdentifierRegex();
    [GeneratedRegex(@"FOREIGN KEY\s*\(\[([^\]]+)\]\)\s+REFERENCES\s+\[([^\]]+)\]\s+\(\[([^\]]+)\]\)", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ForeginKeyRegex();
    [GeneratedRegex(@"CREATE\s+(UNIQUE\s+)?INDEX\s+\[([^\]]+)\]\s+ON\s+\[([^\]]+)\]\s+\(\[([^\]]+)\]\)", RegexOptions.IgnoreCase | RegexOptions.Multiline, "en-US")]
    private static partial Regex IndexRegex();
}

