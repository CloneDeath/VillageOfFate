using VillageOfFate.DAL;
using VillageOfFate.DAL.Entities;

namespace VillageOfFate.Services.DALServices.Core;

public class TimeService(DataContext context) {
	public async Task<DateTime> GetAsync(TimeLabel label, DateTime? defaultValue = null) {
		var entry = await context.Time.FindAsync(label);
		if (entry != null) return entry.Time;

		var now = defaultValue ?? DateTime.UtcNow;
		await context.Time.AddAsync(new TimeDto {
			Label = label,
			Time = now
		});
		await context.SaveChangesAsync();
		return now;
	}

	public async Task SetAsync(TimeLabel label, DateTime value) {
		var entry = await context.Time.FindAsync(label);
		if (entry != null) {
			entry.Time = value.ToUniversalTime();
			context.Time.Update(entry);
		} else {
			await context.Time.AddAsync(new TimeDto {
				Label = label,
				Time = value.ToUniversalTime()
			});
		}

		await context.SaveChangesAsync();
	}
}