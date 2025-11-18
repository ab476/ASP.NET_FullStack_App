using EFCore.NamingConventions.Internal;
using System.Globalization;

namespace Common.Features.NameHelper;
public class NameHelper(INameRewriter nameRewriter) : INameHelper
{
    private readonly INameRewriter _nameRewriter = nameRewriter;

    public string RewriteName(string name)
    {
        return _nameRewriter.RewriteName(name);
    }

    public static INameHelper Default { get; internal set; } 
        = new NameHelper(new SnakeCaseNameRewriter(CultureInfo.CurrentCulture));

    public static string Rewrite(string name)
        => Default.RewriteName(name);
}
