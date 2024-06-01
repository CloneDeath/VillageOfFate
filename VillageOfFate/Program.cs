using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using VillageOfFate.VillagerActions;

namespace VillageOfFate;

public static class Program {
	public static async Task Main(string[] args) {
		var parser = new Parser(with => with.HelpWriter = Console.Error);
		var result = parser.ParseArguments<ProgramOptions>(args);

		// Moving to Server...

		var logger = new VillageLogger(result.Value.LogDirectory ?? Directory.GetCurrentDirectory());
		List<IVillagerAction> actions = [
			new SpeakAction(logger),
			new DoNothingAction(),
			new InteractAction(logger),
			new AdjustEmotionalStateAction(logger),
			new EatAction(logger),
			new SleepAction(logger),
			new LookoutAction(logger)
		];
	}
}