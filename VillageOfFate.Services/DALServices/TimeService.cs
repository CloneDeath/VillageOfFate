using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class TimeService(DataContext dataContext) {
	public async Task<DateTime> GetTime() {
		var entry = await dataContext.Time.FirstOrDefaultAsync();
		if (entry != null) return entry.Now;

		var now = DateTime.UtcNow;
		await dataContext.Time.AddAsync(new Time {
			Now = now
		});
		await dataContext.SaveChangesAsync();
		return now;
	}
}