using Microsoft.EntityFrameworkCore;
using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class TimeService(DataContext dataContext) {
	public async Task<DateTime> GetTimeAsync() {
		var entry = await dataContext.Time.FirstOrDefaultAsync();
		if (entry != null) return entry.Now;

		var now = DateTime.UtcNow;
		await dataContext.Time.AddAsync(new TimeDto {
			Now = now
		});
		await dataContext.SaveChangesAsync();
		return now;
	}

	public async Task SetTimeAsync(DateTime value) {
		var entry = await dataContext.Time.FirstOrDefaultAsync();
		if (entry != null) {
			entry.Now = value.ToUniversalTime();
			dataContext.Time.Update(entry);
		} else {
			await dataContext.Time.AddAsync(new TimeDto {
				Now = value.ToUniversalTime()
			});
		}

		await dataContext.SaveChangesAsync();
	}
}