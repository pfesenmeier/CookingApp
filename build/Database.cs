using System;

namespace Build;

public static class Database
{
    public static string GenerateMigrationFileName(string postfix)
    {
        return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + postfix.Trim() + ".sql";
    }
}
