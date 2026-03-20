public class StringUtils
{
    public List<Char> specialChars = new List<char>()
    {
        S2C(" "),
        S2C("\'"),
        S2C("\""),
        S2C("$"),
        S2C("*"),
        S2C("?"),
        S2C("{"),
        S2C("}"),
        S2C("("),
        S2C(")"),
        S2C("["),
        S2C("]"),
        S2C("<"),
        S2C(">")
    };

    public static Char S2C(string c)
    {
        return Convert.ToChar(c);
    }
    
    public bool HasSpecialMeaning(char c)
    {
        if (specialChars.Contains(c))
            return true;
        
        return false;
    }

    public bool IsDoubleSpecial(string s)
    {
        if (s == "<?" || s == "?>" || s == "<%" || s == "%>" || s == "/*" || s == "*/")
            return true;
        
        return false;
    }
}