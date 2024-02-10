using OneOf;
using OneOf.Types;
using ProductApi.Model.Entities;
using ProductApi.Shared.Model;
using ProductApi.Shared.Model.Responses;

namespace ProductApi.Service.Interfaces;

public interface IFileService {
    Task<ImagesCreateResponse> CreateImagesAsync(Guid productId, Stream fileStream, string contentType);
    Task<ImageDeleteResponse> DeleteImageAsync(Guid productId, Guid fileId);
    ImageGetAllResponse GetUrisForImages(List<Image> images);
}

[GenerateOneOf]
public partial class ImagesCreateResponse : OneOfBase<FileDto, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ImageDeleteResponse : OneOfBase<Success, NotFoundResponse> {
}

[GenerateOneOf]
public partial class ImageGetAllResponse : OneOfBase<List<string>> {
}