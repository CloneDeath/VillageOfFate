using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices;

public class TimeService(DataContext dataContext) {
	public async Task<DateTime> GetAsync(TimeLabel label) {
		var entry = await dataContext.Time.FindAsync(label);
		if (entry != null) return entry.Now;

		var now = DateTime.UtcNow;
		await dataContext.Time.AddAsync(new TimeDto {
			Label = label,
			Now = now
		});
		await dataContext.SaveChangesAsync();
		return now;
	}

	public async Task SetAsync(TimeLabel label, DateTime value) {
		var entry = await dataContext.Time.FindAsync(label);
		if (entry != null) {
			entry.Now = value.ToUniversalTime();
			dataContext.Time.Update(entry);
		} else {
			await dataContext.Time.AddAsync(new TimeDto {
				Label = label,
				Now = value.ToUniversalTime()
			});
		}

		await dataContext.SaveChangesAsync();
	}
}