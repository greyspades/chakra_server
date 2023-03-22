using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Core;
using MongoDB.Driver.GridFS;
using System.IO;


namespace MongoDB.Helpers;

public static class MongoHelpers
{
    public static async Task<ObjectId> UploadFromStreamAsync(IFormFile file, IMongoDatabase db)
    {
        // var stream = new FileStream(file.FileName, FileMode.Open);
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var gridFsBucket = new GridFSBucket(db);

        var task = Task.Run(() => gridFsBucket.UploadFromStreamAsync(file.FileName, stream));
        return task.Result;
    }

    public static async Task<FileStream> DownloadFromStreamAsync(string fileName, IMongoDatabase db)
    {
        var gridFsBucket = new GridFSBucket(db);
        var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, fileName);
        var finData = await gridFsBucket.FindAsync(filter);
        var firstData = finData.FirstOrDefault();
        var bsonId = firstData.Id;
        var content = new FileStream("wwwroot/cv/thefile21.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        // await gridFsBucket.DownloadToStreamAsync(bsonId, content);
        var stream = await gridFsBucket.OpenDownloadStreamAsync(bsonId);
        Console.WriteLine(stream.CanRead);
        Console.WriteLine(stream.CanWrite);
        Console.WriteLine(stream.FileInfo.Filename);
        return content;
    }
}