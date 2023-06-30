namespace UnitOfWorkPatternExample;

public sealed class FileSystem
{
    private readonly string _basePath;

    public FileSystem(string basePath)
    {
        _basePath = basePath;
    }
    
    public async Task WriteAllTextAsync(string file, string content)
    {
        Console.WriteLine("Creating file {0}", file);
        await File.WriteAllTextAsync(_createFilePath(file), content);
    }

    public async Task<string?> TryReadAllTextAsync(string file)
    {
        try
        {
            return await File.ReadAllTextAsync(_createFilePath(file));
        }
        catch
        {
            return null;
        }
    }

    public void Delete(string file)
    {
        if (Exists(file) == false)
        {
            throw new Exception("File doesn't exist so can't delete");
        }
        Console.WriteLine("Deleting file {0}", file);
        File.Delete(_createFilePath(file));
    }

    public bool Exists(string file)
    {
        return File.Exists(_createFilePath(file));
    }

    private string _createFilePath(string fileName)
    {
        return _basePath + "/" + fileName;
    }
}