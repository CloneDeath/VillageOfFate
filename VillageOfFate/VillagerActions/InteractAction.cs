namespace VillageOfFate.VillagerActions;

public class InteractAction : IVillagerAction {
	public string Name => "Interact";
	public string Description => "Interact with someone else";
	public object Parameters => new {
		type = "object",
		properties = new {
			targets = new {
				type = "array",
				items = new {
					type = "string"
				},
				description = "who the action is directed at"
			},
			action = new {
				type = "string",
				description = "A description of the interaction you are performing"
			}
		}
	};
}