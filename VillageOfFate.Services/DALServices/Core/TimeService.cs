using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class TimeService(DataContext dataContext) {
	public async Task<DateTime> GetAsync(TimeLabel label, DateTime? defaultValue = null) {
		var entry = await dataContext.Time.FindAsync(label);
		if (entry != null) return entry.Time;

		var now = defaultValue ?? DateTime.UtcNow;
		await dataContext.Time.AddAsync(new TimeDto {
			Label = label,
			Time = now
		});
		await dataContext.SaveChangesAsync();
		return now;
	}

	public async Task SetAsync(TimeLabel label, DateTime value) {
		var entry = await dataContext.Time.FindAsync(label);
		if (entry != null) {
			entry.Time = value.ToUniversalTime();
			dataContext.Time.Update(entry);
		} else {
			await dataContext.Time.AddAsync(new TimeDto {
				Label = label,
				Time = value.ToUniversalTime()
			});
		}

		await dataContext.SaveChangesAsync();
	}
}