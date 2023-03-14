using MyReliableSite.Application.Storage;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Infrastructure.Common.Extensions;
using MyReliableSite.Shared.DTOs.Storage;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MyReliableSite.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    public string GetBase64Image(FileUploadRequest request)
    {
        return request.Data; // Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
    }

    public Task<string> UploadAsync<T>(FileUploadRequest request, FileType supportedFileType)
        where T : class
    {
        if (request == null || request.Data == null)
        {
            return Task.FromResult(string.Empty);
        }

        if (!supportedFileType.GetDescriptionList().Contains(request.Extension))
            throw new Exception("File Format Not Supported.");

        string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
        if (streamData.Length > 0)
        {
            string folder = typeof(T).Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                folder = folder.Replace(@"\", "/");
            }

            string folderName = supportedFileType switch
            {
                FileType.Image => Path.Combine("Files", "Images", folder),
                _ => Path.Combine("Files", "Others", folder),
            };
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            bool exists = Directory.Exists(pathToSave);
            if (!exists)
            {
                Directory.CreateDirectory(pathToSave);
            }

            string fileName = request.Name.Trim('"');
            fileName = RemoveSpecialCharacters(fileName);
            fileName = fileName.ReplaceWhitespace("-");
            fileName += request.Extension.Trim();
            string timeStamp = GetUnixTimstamp().ToString();
            fileName = string.Concat(timeStamp, fileName);
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(folderName, fileName);
            if (File.Exists(dbPath))
            {
                dbPath = NextAvailableFilename(dbPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var stream = new FileStream(fullPath, FileMode.Create);
            streamData.CopyTo(stream);
            dbPath = dbPath.Replace("\\", "/");
            return Task.FromResult(dbPath);
        }
        else
        {
            return Task.FromResult(string.Empty);
        }
    }

    public ServerFile PrepareFile(FileUploadRequest request)
    {
        if (string.IsNullOrEmpty(request?.Name) || string.IsNullOrEmpty(request?.Extension) || string.IsNullOrEmpty(request?.Data)) return null;
        string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        return (ServerFile)new(request.Name, request.Extension, base64Data);
    }

    public string PrepareDataForImage(ServerFile file)
    {
        if (file == null) return string.Empty;
        return string.Concat("data:image/" + file.FileExtension.Replace(".", string.Empty) + ";base64," + file.Base64Data);
    }

    public Task<string> UploadCSVFileAsync<T>(FileUploadRequest request, FileType supportedFileType)
        where T : class
    {
        if (request == null || request.Data == null)
        {
            return Task.FromResult(string.Empty);
        }

        if (!supportedFileType.GetDescriptionList().Contains(request.Extension))
            throw new Exception("File Format Not Supported.");

        var streamData = new MemoryStream(Convert.FromBase64String(request.Data));
        if (streamData.Length > 0)
        {
            string folder = typeof(T).Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                folder = folder.Replace(@"\", "/");
            }

            string folderName = supportedFileType switch
            {
                FileType.Image => Path.Combine("Files", "Images", folder),
                _ => Path.Combine("Files", "Others", folder),
            };
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            bool exists = Directory.Exists(pathToSave);
            if (!exists)
            {
                Directory.CreateDirectory(pathToSave);
            }

            string fileName = request.Name.Trim('"');
            fileName = RemoveSpecialCharacters(fileName);
            fileName = fileName.ReplaceWhitespace("-");
            fileName += request.Extension.Trim();
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(folderName, fileName);
            if (File.Exists(dbPath))
            {
                dbPath = NextAvailableFilename(dbPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var stream = new FileStream(fullPath, FileMode.Create);
            streamData.CopyTo(stream);
            dbPath = dbPath.Replace("\\", "/");
            return Task.FromResult(dbPath);
        }
        else
        {
            return Task.FromResult(string.Empty);
        }
    }

    public Task<string> ReturnBase64StringOfImageFileAsync(string filepath)
    {
        if (string.IsNullOrEmpty(filepath))
        {
            return Task.FromResult(string.Empty);
        }

        string base64String = string.Empty;
        string pathToDownload = Path.Combine(Directory.GetCurrentDirectory(), filepath);

        if (File.Exists(pathToDownload))
        {
            byte[] bytes = File.ReadAllBytes(pathToDownload);
            base64String = string.Concat("data:image/" + Path.GetExtension(pathToDownload).Replace(".", string.Empty) + ";base64," + Convert.ToBase64String(bytes));
        }
        else
        {
            pathToDownload = pathToDownload.Replace("client", "admin");
            if (File.Exists(pathToDownload))
            {
                byte[] bytes = File.ReadAllBytes(pathToDownload);
                base64String = string.Concat("data:image/" + Path.GetExtension(pathToDownload).Replace(".", string.Empty) + ";base64," + Convert.ToBase64String(bytes));
            }
        }

        return Task.FromResult(base64String);

    }

    private static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
    }

    private const string NumberPattern = "-{0}";

    private static string NextAvailableFilename(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        if (Path.HasExtension(path))
        {
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal), NumberPattern));
        }

        return GetNextFilename(path + NumberPattern);
    }

    private static string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);

        if (!File.Exists(tmp))
        {
            return tmp;
        }

        int min = 1, max = 2;

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
            {
                min = pivot;
            }
            else
            {
                max = pivot;
            }
        }

        return string.Format(pattern, max);
    }

    public static long GetUnixTimstamp()
    {
        return GetUnixTimstamp(DateTime.UtcNow);
    }

    public static long GetUnixTimstamp(DateTime date)
    {
        DateTime zero = new DateTime(1970, 1, 1);
        TimeSpan span = date.Subtract(zero);

        return (long)span.TotalMilliseconds;
    }
}