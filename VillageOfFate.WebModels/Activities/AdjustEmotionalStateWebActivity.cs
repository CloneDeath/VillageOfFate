namespace VillageOfFate.WebModels.Activities;

public class AdjustEmotionalStateWebActivity : WebActivity {
	public required VillagerEmotion Emotion { get; init; }
	public required int Adjustment { get; init; }
	public required string Reason { get; init; }
}