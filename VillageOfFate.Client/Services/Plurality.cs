namespace VillageOfFate.Client.Services;

public class Plurality {
	public virtual string Pick(int count, string singular, string plural) =>
		count == 1 ? singular : plural;
}