using OneOf;
using OneOf.Types;

namespace BlobApi.Interfaces;

public interface IFileService
{
    Task<ImageGetResponse> GetImageByNameAsync(string imageName);
    Task<ImageCreateResponse> CreateImageAsync(Stream fileStream);
    Task<ImageDeleteResponse> DeleteImageAsync(string imageName);

}

[GenerateOneOf]
public partial class ImageCreateResponse : OneOfBase<string>
{
}

[GenerateOneOf]
public partial class ImageDeleteResponse : OneOfBase<Success, NotFound>
{
}

[GenerateOneOf]
public partial class ImageGetResponse : OneOfBase<string, NotFound>
{
}

