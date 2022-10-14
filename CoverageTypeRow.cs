namespace TtxGenerator.net
{
    public class CoverageTypeRow
    {
        public string? PolicyTypeCode { get; set; }
        public string? CoverageTypeCode { get; set; }
        public string? CoverageTypeName { get; set; }
        public string? CoverageSubTypeCode { get; set; }
        public string? CoverageSubTypeName { get; set; }
        public string? ExposureTypeCode { get; set; }
        public string? CostCategoryCode { get; set; }
        public string? CostCategoryName { get; set; }
        public string? CovTermCode { get; set; }
        public string? CovTermIdentifierCode { get; set; }

        public override string ToString()
        {
            return $"{PolicyTypeCode} {CoverageTypeCode} {CoverageSubTypeCode} {ExposureTypeCode} {CostCategoryCode} {CovTermCode}";
        }
    }
}
