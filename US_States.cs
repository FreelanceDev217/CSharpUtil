public class US_States
{
	public static string StateABB2FULL(string state_abb)
	{
		switch (state_abb)
		{
			case "AK":
				return "Alaska";
			case "AL":
				return "Alabama";
			case "AR":
				return "Arkansas";
			case "AZ":
				return "Arizona";
			case "CA":
				return "California";
			case "CO":
				return "Colorado";
			case "CT":
				return "Connecticut";
			case "DC":
				return "District of Columbia";
			case "DE":
				return "Delaware";
			case "FL":
				return "Florida";
			case "GA":
				return "Georgia";
			case "HI":
				return "Hawaii";
			case "IA":
				return "Iowa";
			case "ID":
				return "Idaho";
			case "IL":
				return "Illinois";
			case "IN":
				return "Indiana";
			case "KS":
				return "Kansas";
			case "KY":
				return "Kentucky";
			case "LA":
				return "Louisiana";
			case "MA":
				return "Massachusetts";
			case "MD":
				return "Maryland";
			case "ME":
				return "Maine";
			case "MI":
				return "Michigan";
			case "MN":
				return "Minnesota";
			case "MO":
				return "Missouri";
			case "MS":
				return "Mississippi";
			case "MT":
				return "Montana";
			case "NC":
				return "North Carolina";
			case "ND":
				return "North Dakota";
			case "NE":
				return "Nebraska";
			case "NH":
				return "New Hampshire";
			case "NJ":
				return "New Jersey";
			case "NM":
				return "New Mexico";
			case "NV":
				return "Nevada";
			case "NY":
				return "New York";
			case "OH":
				return "Ohio";
			case "OK":
				return "Oklahoma";
			case "OR":
				return "Oregon";
			case "PA":
				return "Pennsylvania";
			case "RI":
				return "Rhode Island";
			case "SC":
				return "South Carolina";
			case "SD":
				return "South Dakota";
			case "TN":
				return "Tennessee";
			case "TX":
				return "Texas";
			case "UT":
				return "Utah";
			case "VA":
				return "Virginia";
			case "VT":
				return "Vermont";
			case "WA":
				return "Washington";
			case "WI":
				return "Wisconsin";
			case "WV":
				return "West Virginia";
			case "WY":
				return "Wyoming";
			default:
				return state_abb;
		}
	}

	public static string StateFULL2ABB(string state_full)
	{
		switch (state_full)
		{
			case "Alaska":
				return "AK";
			case "Alabama":
				return "AL";
			case "Arkansas":
				return "AR";
			case "Arizona":
				return "AZ";
			case "California":
				return "CA";
			case "Colorado":
				return "CO";
			case "Connecticut":
				return "CT";
			case "DC":
				return "District of Columbia";
			case "DE":
				return "Delaware";
			case "Florida":
				return "FL";
			case "Georgia":
				return "GA";
			case "Hawaii":
				return "HI";
			case "Iowa":
				return "IA";
			case "Idaho":
				return "ID";
			case "Illinois":
				return "IL";
			case "Indiana":
				return "IN";
			case "Kansas":
				return "KS";
			case "Kentucky":
				return "KY";
			case "Louisiana":
				return "LA";
			case "Massachusetts":
				return "MA";
			case "Maryland":
				return "MD";
			case "Maine":
				return "ME";
			case "Michigan":
				return "MI";
			case "Minnesota":
				return "MN";
			case "Missouri":
				return "MO";
			case "Mississippi":
				return "MS";
			case "Montana":
				return "MT";
			case "North Carolina":
				return "NC";
			case "North Dakota":
				return "ND";
			case "Nebraska":
				return "NE";
			case "New Hampshire":
				return "NH";
			case "New Jersey":
				return "NJ";
			case "New Mexico":
				return "NM";
			case "Nevada":
				return "NV";
			case "New York":
				return "NY";
			case "Ohio":
				return "OH";
			case "Oklahoma":
				return "OK";
			case "Oregon":
				return "OR";
			case "Pennsylvania":
				return "PA";
			case "Rhode Island":
				return "RI";
			case "South Carolina":
				return "SC";
			case "South Dakota":
				return "SD";
			case "Tennessee":
				return "TN";
			case "Texas":
				return "TX";
			case "Utah":
				return "UT";
			case "Virginia":
				return "VA";
			case "Vermont":
				return "VT";
			case "Washington":
				return "WA";
			case "Wisconsin":
				return "WI";
			case "West Virginia":
				return "WV";
			case "WY":
				return "Wyoming";
			default:
				return state_full;
		}
	}
}