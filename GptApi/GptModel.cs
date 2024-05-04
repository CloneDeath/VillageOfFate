using System;

namespace GptApi;

public enum GptModel {
	Gpt_35_Turbo,
	Gpt_4_Turbo
}

public static class GptModelExtensions {
	public static string ToModelString(this GptModel model) => model switch {
		GptModel.Gpt_35_Turbo => "gpt-3.5-turbo",
		GptModel.Gpt_4_Turbo => "gpt-4-turbo",
		_ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
	};
}