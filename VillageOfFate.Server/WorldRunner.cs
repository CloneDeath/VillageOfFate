using System;
using System.Threading;
using System.Threading.Tasks;
using VillageOfFate.Services.DALServices;

namespace VillageOfFate.Server;

public class WorldRunner(TimeService time) {
	private readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

	public async Task<DateTime> GetLastUpdateAsync() => await time.GetTimeAsync();
	public async Task SetLastUpdateAsync(DateTime value) => await time.SetTimeAsync(value);

	public async Task RunAsync(CancellationToken cancellationToken) {
		var endTime = await GetLastUpdateAsync() + TimeSpan.FromMinutes(2);

		while (!cancellationToken.IsCancellationRequested) {
			var now = DateTime.UtcNow;
			var timeSinceLastUpdate = now - await GetLastUpdateAsync();

			if (now > endTime) {
				await Task.Delay(Interval, cancellationToken);
			}

			if (timeSinceLastUpdate >= Interval) {
				SimulateWorld();
				await SetLastUpdateAsync(now);
			} else {
				// Sleep until it's time for the next update
				var sleepDuration = Interval - timeSinceLastUpdate;
				await Task.Delay(sleepDuration, cancellationToken);
			}
		}
	}

	private void SimulateWorld() {
		var villager = GetVillagerWithTheShortestCompleteTime(villagers);
		var waitTime = villager.CurrentActivity.EndTime - world.CurrenTime;
		if (villager.CurrentActivity.EndTime > world.CurrenTime) {
			// If the villager's current activity is not yet complete, wait until it is
			Thread.Sleep(waitTime);
			world.CurrenTime += waitTime;
		}

		var activityResult = villager.CurrentActivity.OnCompletion();
		if (villager.ActivityQueue.TryPop(out var activity)) {
			activity.StartTime = world.CurrenTime;
			villager.CurrentActivity = activity;
		} else {
			await QueueActionsForVillager(villager, world, chatGptApi, actions, logger, villagers);
			PushCurrentActivityIntoQueue(villager, world);
			villager.CurrentActivity = new IdleActivity(random.NextTimeSpan(TimeSpan.FromMinutes(2)), world);
		}

		if (activityResult.TriggerReactions.Any()) {
			var selected = random.SelectOne(activityResult.TriggerReactions);
			PushCurrentActivityIntoQueue(selected, world);
			await QueueActionsForVillager(selected, world, chatGptApi, actions, logger, villagers);
		}
	}
}