using UnitOfWorkPatternExample;

var filesDirectory = Directory.GetCurrentDirectory() + "/Files";
var fileSystem = new FileSystem(filesDirectory);

// Successfully create 3 files
Console.WriteLine("- Creating the first three files");
var fileSystemUnitOfWork = new FileSystemUnitOfWork(fileSystem);
fileSystemUnitOfWork.Add("file1", "content");
fileSystemUnitOfWork.Add("file2", "content");
fileSystemUnitOfWork.Add("file3", "content");
await fileSystemUnitOfWork.ExecuteAsync();

// Fail on deleting the last file then rollback
Console.WriteLine("- Creating two files and attempting to delete three files");
fileSystemUnitOfWork.Add("file4", "content");
fileSystemUnitOfWork.Add("file5", "content");
await fileSystemUnitOfWork.DeleteAsync("file1");
await fileSystemUnitOfWork.DeleteAsync("file2");
await fileSystemUnitOfWork.DeleteAsync("file3");
    
// Now lets simulate a failure that could happen (file doesnt exist for file3).
File.Delete(filesDirectory + "/file3");

try
{
    await fileSystemUnitOfWork.ExecuteAsync();
}
catch (Exception e)
{
    Console.WriteLine("Execution failed because {0}", e.Message);
}