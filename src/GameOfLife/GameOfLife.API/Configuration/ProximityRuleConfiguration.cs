namespace GameOfLife.API.Configuration
{
    /// <summary>
    /// Configuration for a proximity rule that can be deserialized from appsettings.json
    /// </summary>
    public class ProximityRuleConfiguration
    {
        /// <summary>
        /// The type of rule: "Simple" for exact match, "Range" for inclusive range
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// For Simple rules: the exact neighbor count required
        /// For Range rules: the minimum neighbor count
        /// </summary>
        public byte Value { get; set; }

        /// <summary>
        /// For Range rules only: the maximum neighbor count
        /// </summary>
        public byte? MaxValue { get; set; }
    }
}
