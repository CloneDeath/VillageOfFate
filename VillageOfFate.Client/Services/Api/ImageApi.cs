using System;

namespace VillageOfFate.Client.Services.Api;

public class ImageApi(string baseUrl) {
	public string GetImageUrl(Guid? imageId) => imageId != null ? $"{baseUrl}Images/{imageId}" : string.Empty;
}