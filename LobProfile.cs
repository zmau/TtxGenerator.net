namespace TtxGenerator.net
{
    public class LobProfile
    {

        public string SpreadsheetFileName { get; set; }
        public string TabName { get; set; }
        public string? PolicyTypeCodeColumn { get; set; }
        public string? CoverageTypeCodeColumn { get; set; }
        public string? CoverageTypeNameColumn { get; set; }
        public string? CoverageSubTypeCodeColumn { get; set; }
        public string? CoverageSubTypeNameColumn { get; set; }
        public string? ExposureTypeCodeColumn { get; set; }
        public string? CostCategoryCodeColumn { get; set; }
        public string? CostCategoryNameColumn { get; set; }
        public string? CovTermCodeColumn { get; set; }
        public string? CovTermNameColumn { get; set; }

        public static LobProfile PROPERTY = new LobProfile
        {
            SpreadsheetFileName = "LOB mapping - Property.xlsx",
            TabName = "Property LOB Mapping",
            PolicyTypeCodeColumn ="E",
            CoverageTypeCodeColumn = "I",
            CoverageTypeNameColumn = "H",
            CoverageSubTypeCodeColumn = "K",
            CoverageSubTypeNameColumn = "J",
            ExposureTypeCodeColumn = "L",
            CostCategoryCodeColumn = "O",
            CostCategoryNameColumn = "N",
            CovTermCodeColumn = "P",
        };
        public static LobProfile LIABILITY = new LobProfile
        {
            SpreadsheetFileName = "LOB mapping - Liability.xlsx",
            TabName = "Liability LOB Mapping",
            PolicyTypeCodeColumn = "E",
            CoverageTypeCodeColumn = "H",
            CoverageTypeNameColumn = "G",
            CoverageSubTypeCodeColumn = "J",
            CoverageSubTypeNameColumn = "I",
            ExposureTypeCodeColumn = "K",
            CostCategoryCodeColumn = "N",
            CostCategoryNameColumn = "M",
            CovTermCodeColumn = "O"
        };
    }
}
