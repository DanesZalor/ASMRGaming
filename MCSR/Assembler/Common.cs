namespace Assembler{
using System.Text.RegularExpressions;

/// <summary> Contains common stuff </summary>
public static class Common
{
    /// <summary> get matches from line that satisfy the grammar pattern </summary>
    public static Match getMatch(string line, string pattern, bool exact = false)
    {
        if (exact) pattern = "^" + pattern + "$";
        try{
            return Regex.Match(line, pattern, RegexOptions.IgnoreCase);
        }catch(System.TypeLoadException e){
            return Match.Empty;
        }
    }

    /// <summary> returns true if line satisfy the grammar pattern </summary>
    public static bool match(string line, string pattern, bool exact = false)
    {
        return getMatch(line, pattern, exact).Success;
    }

    public static string replace(string line, string pattern, string replacement){

        try{
            return Regex.Replace(line, pattern, replacement, RegexOptions.IgnoreCase);
        }catch(System.TypeLoadException){
            return "";
        }
        
    }
}

}
