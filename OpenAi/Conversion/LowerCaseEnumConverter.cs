using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAi.Conversion;

public class LowerCaseEnumConverter() : JsonStringEnumConverter(new LowerCaseNamingPolicy());

public class LowerCaseNamingPolicy : JsonNamingPolicy {
	public override string ConvertName(string name) => name.ToLower();
}