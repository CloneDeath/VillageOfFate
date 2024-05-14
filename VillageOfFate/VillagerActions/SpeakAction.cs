namespace VillageOfFate.VillagerActions;

public class SpeakAction : IVillagerAction {
	public string Name => "Speak";
	public string Description => "Say something";
	public object Parameters => new {
		type = "object",
		properties = new {
			content = new {
				type = "string",
				description = "what to say"
			}
		}
	};
}