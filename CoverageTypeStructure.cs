using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TtxGenerator.net
{

    internal class CovTermStruct
    {
        public string? CovTermCode { get; set; }
        public string? CovTermCodeFromTtx { get; set; }
        public string? CovTermIdentifierCode { get; set; }
        public string? WholeTag { get; set; }

        public override string ToString()
        {
            return $"           covTerm {CovTermCode} {CovTermIdentifierCode}";
        }
    }
    internal class CostCategoryStruct
    {
        public string? CostCategoryCode { get; set; }
        public string? CostCategoryName { get; set; }
        public List<CovTermStruct>? CovTermStruct { get; set; }

        public CostCategoryStruct()
        {
            CovTermStruct = new List<CovTermStruct>();
        }

        public override string ToString()
        {
            return $"       costCat {CostCategoryCode}";
        }
    }
    internal class CoverageSubTypeStruct
    {
        public string? CoverageTypeCode { get; set; }
        public string? CoverageSubTypeCode { get; set; }
        public string? CoverageSubTypeName { get; set; }

        public string? ExposureTypeCode { get; set; }

        public List<CostCategoryStruct>? CostCategories { get; set; }

        public CoverageSubTypeStruct()
        {
            CostCategories = new List<CostCategoryStruct>();
        }

        public override string ToString()
        {
            return $"       subtype {CoverageSubTypeCode}";
        }
    }

    internal class CoverageTypeStruct
    {
        public string? PolicyTypeCode { get; set; }
        public string? CoverageTypeCode { get; set; }
        public string? CoverageTypeName { get; set; }

        private List<CoverageSubTypeStruct> _coverageSubTypes;
        public List<CoverageSubTypeStruct> CoverageSubTypes { get { return _coverageSubTypes; } }
        public CoverageTypeStruct()
        {
            _coverageSubTypes = new List<CoverageSubTypeStruct>();
        }

        public override string ToString()
        {
            return $"   type {CoverageTypeCode}";
        }

    }
}
