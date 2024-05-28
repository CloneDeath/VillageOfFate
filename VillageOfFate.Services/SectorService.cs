using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;
using VillageOfFate.WebModels;

namespace VillageOfFate.Services;

public class SectorService(DataContext context) {
	public async Task<SectorDto> GetSector(Position position) {
		var sector = await context.Sectors.FirstOrDefaultAsync(s => s.X == position.X && s.Y == position.Y);
		if (sector != null) {
			return sector;
		}

		var result = await context.Sectors.AddAsync(new SectorDto {
			Description =
				"A dense, lush forest filled with towering trees, diverse wildlife, and the sounds of nature. " +
				"It's easy to lose one's way in this vast sea of green.",
			X = position.X,
			Y = position.Y
		});
		await context.SaveChangesAsync();
		return result.Entity;
	}
}