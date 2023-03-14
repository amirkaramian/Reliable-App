using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Shared.DTOs.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace MyReliableSite.Application.Storage;

public interface IFileStorageService : ITransientService
{
    public Task<string> UploadAsync<T>(FileUploadRequest request, FileType supportedFileType)
        where T : class;
    Task<string> UploadCSVFileAsync<T>(FileUploadRequest request, FileType supportedFileType)
        where T : class;

    Task<string> ReturnBase64StringOfImageFileAsync(string filepath);
    ServerFile PrepareFile(FileUploadRequest request);
    string PrepareDataForImage(ServerFile file);

    string GetBase64Image(FileUploadRequest request);
}