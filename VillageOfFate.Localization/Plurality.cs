namespace VillageOfFate.Localization;

public class Plurality {
	public virtual string Pick(int count, string singular, string plural) => count == 1 ? singular : plural;
	public virtual string Pick<T>(IEnumerable<T> items, string singular, string plural) => items.Count() == 1 ? singular : plural;
}