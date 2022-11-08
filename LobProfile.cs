namespace TtxGenerator.net
{
    public class LobProfile
    {

        public string SpreadsheetFileName { get; set; }

        public string LOBMappingTabName { get; set; }
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

        public string ISOMappingTabName { get; set; }

        public string? CC_CoverageTypeColumn { get; set; }
        public string? CC_CoverageSubTypeColumn { get; set; }
        public string? ISO_PolicyTypeColumn { get; set; }
        public string? ISO_CoverageTypeColumn { get; set; }
        public string? ISO_LossTypeColumn { get; set; }

        public static LobProfile PROPERTY = new LobProfile
        {
            SpreadsheetFileName = "LOB mapping - Property.xlsx",
            LOBMappingTabName = "Property LOB Mapping",
            PolicyTypeCodeColumn ="E",
            CoverageTypeCodeColumn = "I",
            CoverageTypeNameColumn = "H",
            CoverageSubTypeCodeColumn = "K",
            CoverageSubTypeNameColumn = "J",
            ExposureTypeCodeColumn = "L",
            CostCategoryCodeColumn = "O",
            CostCategoryNameColumn = "N",
            CovTermCodeColumn = "P",

            ISOMappingTabName = "ISO Mapping",
            CC_CoverageTypeColumn = "C",
            CC_CoverageSubTypeColumn = "D",
            ISO_PolicyTypeColumn = "E",
            ISO_CoverageTypeColumn= "F",
            ISO_LossTypeColumn = "G"
        };
        public static LobProfile LIABILITY = new LobProfile
        {
            SpreadsheetFileName = "LOB mapping - Liability.xlsx",
            LOBMappingTabName = "Liability LOB Mapping",
            PolicyTypeCodeColumn = "E",
            CoverageTypeCodeColumn = "H",
            CoverageTypeNameColumn = "G",
            CoverageSubTypeCodeColumn = "J",
            CoverageSubTypeNameColumn = "I",
            ExposureTypeCodeColumn = "K",
            CostCategoryCodeColumn = "N",
            CostCategoryNameColumn = "M",
            CovTermCodeColumn = "O",

            ISOMappingTabName = "ISO Mapping",
            CC_CoverageTypeColumn = "C",
            CC_CoverageSubTypeColumn = "D",
            ISO_PolicyTypeColumn = "E",
            ISO_CoverageTypeColumn = "F",
            ISO_LossTypeColumn = "G"
        };
    }
}
