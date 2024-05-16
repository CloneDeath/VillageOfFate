using CommandLine;

namespace VillageOfFate;

public class ProgramOptions {
	[Option('l', "logDirectory", Required = false, HelpText = "Set the log directory.")]
	public string? LogDirectory { get; set; }
}