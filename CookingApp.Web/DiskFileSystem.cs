using HandlebarsDotNet;

namespace CookingApp.Web;

public class DiskFileSystem : ViewEngineFileSystem
{
    public override string GetFileContent(string filename)
    {
        return File.ReadAllText(filename);
    }

    protected override string CombinePath(string dir, string otherFileName)
    {
        return Path.Combine(dir, otherFileName);
    }

    public override bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}
