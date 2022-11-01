using System.Collections.Generic;
using System;

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

    class CoverageTypeRowSubtypeComparer : IEqualityComparer<CoverageTypeRow>
    {
        public bool Equals(CoverageTypeRow x, CoverageTypeRow y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.CoverageSubTypeCode == y.CoverageSubTypeCode;
        }

        public int GetHashCode(CoverageTypeRow row)
        {
            if (ReferenceEquals(row, null)) return 0;
            int subTypeHash = row.CoverageSubTypeCode == null ? 0 : row.CoverageSubTypeCode.GetHashCode();
            return subTypeHash;
        }
    }
    class CoverageTypeRowCostCategoryComparer : IEqualityComparer<CoverageTypeRow>
    {
        public bool Equals(CoverageTypeRow x, CoverageTypeRow y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.CoverageSubTypeCode == y.CoverageSubTypeCode && x.CostCategoryCode == y.CostCategoryCode;
        }

        public int GetHashCode(CoverageTypeRow row)
        {
            if (ReferenceEquals(row, null)) return 0;
            int subTypeHash = row.CoverageSubTypeCode == null ? 0 : row.CoverageSubTypeCode.GetHashCode();
            int costCategoryHash = row.CostCategoryCode == null ? 0 : row.CostCategoryCode.GetHashCode();
            return subTypeHash ^ costCategoryHash;
        }
    }
    class CoverageTypeRowCovTermPatternComparer : IEqualityComparer<CoverageTypeRow>
    {
        public bool Equals(CoverageTypeRow x, CoverageTypeRow y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.CoverageSubTypeCode == y.CoverageSubTypeCode && x.CostCategoryCode == y.CostCategoryCode && x.CovTermCode == y.CovTermCode;
        }

        public int GetHashCode(CoverageTypeRow row)
        {
            if (ReferenceEquals(row, null)) return 0;
            int subTypeHash = row.CoverageSubTypeCode == null ? 0 : row.CoverageSubTypeCode.GetHashCode();
            int costCategoryHash = row.CostCategoryCode == null ? 0 : row.CostCategoryCode.GetHashCode();
            int covTermPatternHash = row.CovTermCode == null ? 0 : row.CovTermCode.GetHashCode();
            return subTypeHash ^ costCategoryHash ^ covTermPatternHash;
        }
    }
}