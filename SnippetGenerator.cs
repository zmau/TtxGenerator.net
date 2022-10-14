using System.Text;

namespace TtxGenerator.net
{

    internal class SnippetGenerator
    {
        private CoverageTypeStruct _structure;
        private StringBuilder _subTypeSnippet;
        private StringBuilder _exposureTypeSnippet;
        private StringBuilder _costCategorySnippet;
        private StringBuilder _covTermPatternSnippet;

        private static readonly string COVTERM_PATTERN_TTX_FILENAME = "C:\\dev\\bmic\\TtxGenerator.net\\CovTermPattern.ttx";
        private static readonly string COVERAGE_TYPE_TTX_FILENAME = "C:\\dev\\bmic\\TtxGenerator.net\\CoverageType.ttx";
        private static readonly string COST_CATEGORY_LINK = "<category code=\"{0}\" typelist=\"CostCategory\"/>";
        private static readonly string CLOSING_TAG = "</typecode>";

        private string _coverageTypeCode;
        public SnippetGenerator(List<CoverageTypeRow> rowList, string coverageTypeCode)
        {
            _structure = new CoverageTypeStruct();
            _structure.PolicyTypeCode = rowList[0].PolicyTypeCode;
            _structure.CoverageTypeCode = rowList[0].CoverageTypeCode;
            _structure.CoverageTypeName = rowList[0].CoverageTypeName;
            //CoverageType desc iz ttx fajla!
            foreach (var row in rowList.DistinctBy(r => r.CoverageSubTypeCode)){
                CoverageSubTypeStruct subType = new CoverageSubTypeStruct();
                subType.CoverageTypeCode = row.CoverageTypeCode;
                subType.CoverageSubTypeCode = row.CoverageSubTypeCode.Replace("GL", "GL1");
                subType.CoverageSubTypeName = row.CoverageSubTypeName;
                subType.ExposureTypeCode = row.ExposureTypeCode;
                var costCategoriesForSubType = rowList.Where(r => r.CoverageSubTypeCode == row.CoverageSubTypeCode);
                costCategoriesForSubType = costCategoriesForSubType.DistinctBy(r => r.CostCategoryCode);
                foreach (var costCategoryRow in costCategoriesForSubType)
                {
                    CostCategoryStruct costCategoryStruct = new CostCategoryStruct();
                    costCategoryStruct.CostCategoryCode = costCategoryRow.CostCategoryCode.Replace("GL", "GL1");
                    costCategoryStruct.CostCategoryName = costCategoryRow.CostCategoryName;
                    var rowTermPatternsForCostCategory = rowList.Where(r => r.CostCategoryCode == costCategoryRow.CostCategoryCode);
                    rowTermPatternsForCostCategory = rowTermPatternsForCostCategory.DistinctBy(r => r.CovTermCode);
                    foreach (var rowForCostCategory in rowTermPatternsForCostCategory)
                    {
                        CovTermStruct covTermStruct = new CovTermStruct();
                        covTermStruct.CovTermCode = rowForCostCategory.CovTermCode;
                        string identifierCode, codeFromTtx;
                        covTermStruct.WholeTag = getCovTermPatternTag(covTermStruct.CovTermCode, rowForCostCategory.CostCategoryCode.Replace("GL", "GL1"), out identifierCode, out codeFromTtx);
                        covTermStruct.CovTermIdentifierCode = identifierCode;
                        covTermStruct.CovTermCodeFromTtx = codeFromTtx;
                        costCategoryStruct.CovTermStruct.Add(covTermStruct);
                    }
                    subType.CostCategories.Add(costCategoryStruct);
                }
                _structure.CoverageSubTypes.Add(subType);
            }
            _exposureTypeSnippet = new StringBuilder();
            _costCategorySnippet = new StringBuilder();
            _subTypeSnippet = new StringBuilder();
            _covTermPatternSnippet = new StringBuilder();

            _coverageTypeCode = coverageTypeCode;
        }

        private readonly string _policyTypeSnippetTemplate = "  <category "
            + "\n    code=\"{0}\" "
            + "\n    typelist=\"CoverageType\"/>\n";

        public string CoverageSubtypeSnippet
        {
            get
            {
                return $"\n CoverageSubtype : {_subTypeSnippet}";
            }
        }

        public string ExposureTypeSnippet
        {
            get
            {
                return $"\n ExposureType : \n{_exposureTypeSnippet}";
            }
        }

        public string CostCategorySnippet
        {
            get
            {
                return $"\n CostCategory : {_costCategorySnippet}";
            }
        }

        public string CovTermPatternSnippet
        {
            get
            {
                return $"\n CovTermPattern : \n {_covTermPatternSnippet}";
            }
        }
        public string PolicyTypeSnippet
        {
            get
            {
                return string.Format(_policyTypeSnippetTemplate, _structure.CoverageTypeCode);
            }
        }

        public string CoverageTypeSnippet
        {
            get
            {
                var subTypeCategoryTags = new StringBuilder();
                foreach(var subType in _structure.CoverageSubTypes)
                {
                    subTypeCategoryTags.Append($"\n    <category"
                        + $"\n      code=\"{subType.CoverageSubTypeCode}\" "
                        + $"\n      typelist = \"CoverageSubtype\" />");
                }
                return $"  <typecode code=\"{_structure.CoverageTypeCode}\" "
                    + $"\n      desc=\"{getCoverageTypeDescFromFile()}\" "
                    + $"\n      identifierCode=\"{_structure.CoverageTypeCode}\" "
                    + $"\n      name=\"{_structure.CoverageTypeName}\">"
                    + "\n    <category "
                    + "\n       code=\"BCP\" "
                    + "\n       typelist=\"PolicyType\"/> "
                    + "\n    <category "
                    + "\n       code=\"PC\" "
                    + "\n       typelist=\"SourceSystem\"/> "
                    + subTypeCategoryTags.ToString()
                    + "\n  </typecode>";
            }
        }

        private string getCoverageTypeDescFromFile()
        {
            List<string> ttxLines = File.ReadAllLines(COVERAGE_TYPE_TTX_FILENAME).ToList();
            bool foundCovTermPattern = false; int lineNo = 0;
            while (!foundCovTermPattern)
            {
                if (ttxLines[lineNo].Contains(_coverageTypeCode))
                    foundCovTermPattern = true;
                else lineNo++;
            }
            return getCoverageTypeDesc(ttxLines[lineNo]);
        }

        private string getCoverageTypeDesc(string coverageTypeLine)
        {
            int descIndex = coverageTypeLine.IndexOf("desc");
            int openingQuoteIndex = descIndex + 1 + coverageTypeLine.Substring(descIndex).IndexOf("\"");
            int descLen = coverageTypeLine.Substring(openingQuoteIndex + 2).IndexOf("\"") + 2;
            string desc = coverageTypeLine.Substring(openingQuoteIndex, descLen);
            return desc;
        }

        public void GenerateCoverageSubTypeSnippet()
        {
            foreach (var subType in _structure.CoverageSubTypes)
            {
                var subTypeTag = "   <typecode"
                + $"\n      code=\"{subType.CoverageSubTypeCode}\" "
                + $"\n      name=\"{subType.CoverageSubTypeName}\" "
                + $"\n      desc=\"{subType.CoverageSubTypeName}\">"
                + "\n       <category"
                + $"\n          code=\"{_structure.CoverageTypeCode}\" "
                + "\n          typelist=\"CoverageType\"/>"
                + "\n       <category"
                + $"\n          code=\"{subType.ExposureTypeCode}**look for something similar manually**\" "
                + "\n          typelist=\"ExposureType\" /> "
                + "\n   </typecode>";
                _subTypeSnippet.Append($"\n{subTypeTag}");
                GenerateExposureTypeSnippet(subType.CoverageSubTypeCode);
                GenerateCostCategorySnippet(subType);
            }
        }

        private void GenerateExposureTypeSnippet(string coverageSubTypeCode)
        {
            var exposureTypeTag = "\n <category"
               + $"\n      code=\"{coverageSubTypeCode}\" "
               + "\n      typelist=\"CoverageSubtype\"/> ";
            _exposureTypeSnippet.Append(exposureTypeTag);
        }

        public void GenerateCostCategorySnippet(CoverageSubTypeStruct subType)
        { 
            var costCategoryTags = new StringBuilder();
            foreach (var costCategory in subType.CostCategories)
            {   // if CostCategoryCode already exists, just add categories to it!
                costCategoryTags.Append("\n   <typecode"
                + $"\n     code=\"{costCategory.CostCategoryCode}\" "
                + $"\n     desc=\"{costCategory.CostCategoryName}\" "
                + $"\n     name=\"{costCategory.CostCategoryName}\" > "
                + $"\n     <category "
                + $"\n       code=\"claimcost\" "
                + $"\n       typelist=\"CostType\"/> "
                + $"\n     <category "
                + $"\n       code=\"{subType.CoverageTypeCode}\" "
                + $"\n       typelist=\"CoverageType\"/> "
                + $"\n     <category "
                + $"\n       code=\"{subType.CoverageSubTypeCode}\" "
                + $"\n       typelist=\"CoverageSubtype\"/> ");
                foreach (var comTermPattern in costCategory.CovTermStruct)
                {
                    costCategoryTags.Append(
                        $"\n     <category "
                    + $"\n       code = \"{comTermPattern.CovTermCodeFromTtx}\" check! "
                    + $"\n       typelist = \"CovTermPattern\"/> ");

                    _covTermPatternSnippet.Append($"\n{comTermPattern.WholeTag}");
                }
                costCategoryTags.Append("\n   </typecode>  ");
            }
            _costCategorySnippet.Append($"\n{costCategoryTags}");
        }

        private string getCovTermPatternTag(string covTermPatternCode, string costCategoryCode, out string identifier, out string codeFromTtx)
        {   // if covTermPatternCode already exist, just add costCategoryCode tag to it!
            List<string> ttxLines = File.ReadAllLines(COVTERM_PATTERN_TTX_FILENAME).ToList();
            bool foundCovTermPattern = false; int lineNo = 0;
            while (!foundCovTermPattern)
            {
                if (ttxLines[lineNo].Contains(covTermPatternCode))
                    foundCovTermPattern = true;
                else lineNo++;
            }
            identifier = getCovTermIdentifierCode(ttxLines[lineNo]);
            codeFromTtx = getCovTermCodeFromTtx(ttxLines[lineNo]);
            StringBuilder ttxTag = new StringBuilder();
            while (!ttxLines[lineNo].Contains(CLOSING_TAG))
            {
                ttxTag.AppendLine(ttxLines[lineNo]);
                lineNo++;
            }
            ttxTag.AppendLine(string.Format(COST_CATEGORY_LINK, costCategoryCode));
            ttxTag.AppendLine(ttxLines[lineNo]);
            
            return ttxTag.ToString();
        }

        private string getCovTermIdentifierCode(string covTermPatternLine)
        {
            List<string> attributes = covTermPatternLine.Split(" ").ToList();
            var identifierCodeAttribute = attributes.Single(att => att.Contains("identifierCode"));
            var nameValue = identifierCodeAttribute.Split("=");
            var identifierCodeValue = nameValue[1].Replace("\"", "");
            return identifierCodeValue;
        }
        private string getCovTermCodeFromTtx(string covTermPatternLine)
        {
            List<string> attributes = covTermPatternLine.Split(" ").ToList();
            var identifierCodeAttribute = attributes.Single(att => att.StartsWith("code"));
            var nameValue = identifierCodeAttribute.Split("=");
            var identifierCodeValue = nameValue[1].Replace("\"", "");
            return identifierCodeValue;
        }
    }
}
