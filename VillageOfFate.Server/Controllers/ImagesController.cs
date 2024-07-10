using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VillageOfFate.Server.Exceptions;
using VillageOfFate.Services.DALServices;

namespace VillageOfFate.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ImagesController(ImageService images) : ControllerBase {
	[HttpGet("{id:guid}")]
	public async Task<FileContentResult> GetImage(Guid id) {
		var image = await images.GetAsync(id);
		if (string.IsNullOrEmpty(image.Base64Image)) {
			throw new AcceptedException("Image is not yet available.");
		}

		var data = Convert.FromBase64String(image.Base64Image);
		return File(data, "image/png");
	}
}