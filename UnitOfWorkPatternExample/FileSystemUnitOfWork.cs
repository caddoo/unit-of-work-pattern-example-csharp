namespace UnitOfWorkPatternExample;

public sealed class FileSystemUnitOfWork
{
    private readonly FileSystem _fileSystem;
    
    private Dictionary<string, string> _fileContentBuffer = new();
    private Dictionary<string, string> _deleteBuffer = new();
    
    public FileSystemUnitOfWork(FileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public void Add(string fileName, string content)
    {
        if (_fileSystem.Exists(fileName))
        {
            throw new Exception("File exists already");
        }

        _fileContentBuffer[fileName] = content;
    }

    public async Task DeleteAsync(string fileName)
    {
        if (_fileContentBuffer.ContainsKey(fileName))
        {
            _fileContentBuffer.Remove(fileName);
            return;
        }

        var fileContent = await _fileSystem.TryReadAllTextAsync(fileName);
        
        if (fileContent == null)
        {
            throw new Exception("File doesn't exist or not readable");
        }
        
        _deleteBuffer.Add(fileName, fileContent);
    }

    public async Task ExecuteAsync()
    {
        try
        {
            foreach (var fileName in _fileContentBuffer.Keys)
            {
                var fileContent = _fileContentBuffer[fileName];
                await _fileSystem.WriteAllTextAsync(fileName, fileContent);
            }

            foreach (var fileName in _deleteBuffer.Keys)
            {
                _fileSystem.Delete(fileName);
            }
        }
        catch
        {
            await _rollbackAsync();
            throw;
        }

        _fileContentBuffer = new Dictionary<string, string>();
        _deleteBuffer = new Dictionary<string, string>();
    }

    private async Task _rollbackAsync()
    {
        Console.WriteLine("Performing rollback");
        foreach (var fileName in _fileContentBuffer.Keys)
        {
            if (_fileSystem.Exists(fileName))
            {
                _fileSystem.Delete(fileName);
            }
        }

        foreach (var fileName in _deleteBuffer.Keys)
        {
            if (_fileSystem.Exists(fileName) == false)
            {
                await _fileSystem.WriteAllTextAsync(fileName, _deleteBuffer[fileName]);
            }
        }
    }
}