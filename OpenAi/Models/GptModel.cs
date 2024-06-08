using System;

namespace OpenAi.Models;

public enum GptModel {
	Gpt_35_Turbo,
	Gpt_4_Turbo,
	Gpt_4_Omni
}

public static class GptModelExtensions {
	public static string ToModelString(this GptModel model) => model switch {
		GptModel.Gpt_35_Turbo => "gpt-3.5-turbo",
		GptModel.Gpt_4_Turbo => "gpt-4-turbo",
		GptModel.Gpt_4_Omni => "gpt-4o",
		_ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
	};
}