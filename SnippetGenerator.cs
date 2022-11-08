using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace TtxGenerator.net
{

    internal class SnippetGenerator
    {
        private CoverageTypeStruct _structure;
        
        private string _coverageTypeSnippet;
        private StringBuilder _subTypeSnippet;
        private StringBuilder _exposureTypeSnippet;
        private StringBuilder _lossPartyTypeSnippet;
        private StringBuilder _costCategorySnippet;
        private StringBuilder _covTermPatternSnippet;
        private StringBuilder _isoMappingSnippet;

        private static readonly string COVERAGE_TYPE_TTX_FILENAME = "CoverageType.ttx";
        private static readonly string COVTERM_PATTERN_TTX_FILENAME = "CovTermPattern.ttx";

        private string _coverageTypeCode;
        private string _inputPath;
        private List<ISOMappingItem> _isoMappingList;
        public SnippetGenerator(List<CoverageTypeRow> rowList, List<ISOMappingItem> isoMappingList, string coverageTypeCode, string inputPath)
        {
            _isoMappingList = isoMappingList;
            _inputPath = inputPath;
            _structure = new CoverageTypeStruct();
            _structure.PolicyTypeCode = rowList[0].PolicyTypeCode;
            _structure.CoverageTypeCode = rowList[0].CoverageTypeCode;
            _structure.CoverageTypeName = rowList[0].CoverageTypeName;
            //CoverageType desc from ttx file!
            foreach (var row in rowList.Distinct(new CoverageTypeRowSubtypeComparer())){
                CoverageSubTypeStruct subType = new CoverageSubTypeStruct();
                subType.CoverageTypeCode = row.CoverageTypeCode;
                subType.CoverageSubTypeCode = row.CoverageSubTypeCode.Replace("GL", "GL1");
                subType.CoverageSubTypeName = row.CoverageSubTypeName;
                subType.ExposureTypeCode = row.ExposureTypeCode;
                var costCategoriesForSubType = rowList
                    .Where(r => r.CoverageSubTypeCode == row.CoverageSubTypeCode)
                    .Distinct(new CoverageTypeRowCostCategoryComparer());
                foreach (var costCategoryRow in costCategoriesForSubType)
                {
                    CostCategoryStruct costCategoryStruct = new CostCategoryStruct();
                    costCategoryStruct.CostCategoryCode = costCategoryRow.CostCategoryCode.Replace("GL", "GL1");
                    costCategoryStruct.CostCategoryName = costCategoryRow.CostCategoryName;
                    var rowTermPatternsForCostCategory = rowList.Where(r => r.CostCategoryCode == costCategoryRow.CostCategoryCode);
                    rowTermPatternsForCostCategory = rowTermPatternsForCostCategory.Distinct(new CoverageTypeRowCovTermPatternComparer());
                    foreach (var rowForCostCategory in rowTermPatternsForCostCategory)
                    {
                        CovTermStruct covTermStruct = new CovTermStruct();
                        covTermStruct.CovTermCode = rowForCostCategory.CovTermCode;
                        string identifierCode, codeFromTtx;
                        covTermStruct.WholeTag = getCovTermPatternTagFromXml(covTermStruct.CovTermCode, rowForCostCategory.CostCategoryCode.Replace("GL", "GL1"), out identifierCode, out codeFromTtx);
                        covTermStruct.CovTermCodeFromTtx = codeFromTtx;
                        costCategoryStruct.CovTermStruct.Add(covTermStruct);
                    }
                    subType.CostCategories.Add(costCategoryStruct);
                }
                _structure.CoverageSubTypes.Add(subType);
            }
            _exposureTypeSnippet = new StringBuilder();
            _lossPartyTypeSnippet = new StringBuilder();
            _costCategorySnippet = new StringBuilder();
            _subTypeSnippet = new StringBuilder();
            _covTermPatternSnippet = new StringBuilder();
            _isoMappingSnippet = new StringBuilder();

            _coverageTypeCode = coverageTypeCode;
        }


        public void GenerateAndWriteAll()
        {
            Console.WriteLine(string.Format(_policyTypeSnippetTemplate, _structure.CoverageTypeCode));

            GenerateCoverageTypeSnippet();
            Console.WriteLine($"\n CoverageType : {_coverageTypeSnippet}");

            GenerateCoverageSubTypeSnippet();
            Console.WriteLine($"\n CoverageSubtype : {_subTypeSnippet}");
            Console.WriteLine($"\n ExposureType : {_exposureTypeSnippet}");
            Console.WriteLine($"\n LossPartyType : {_lossPartyTypeSnippet}");
            Console.WriteLine($"\n CostCategory : {_costCategorySnippet}");
            Console.WriteLine($"\n CovTermPattern : \n {_covTermPatternSnippet}");

            GenerateISOMappingSnippet();
            Console.WriteLine($"\n ISO mapping : \n{_isoMappingSnippet}");

            Console.WriteLine("\n\n\n\npress ENTER to close console");
            Console.ReadLine();
        }

        private readonly string _policyTypeSnippetTemplate = "PolicyType : \n   <category "
            + "\n    code=\"{0}\" "
            + "\n    typelist=\"CoverageType\"/>\n";

        private void GenerateCoverageTypeSnippet()
        {
            var subTypeCategoryTags = new StringBuilder();
            foreach(var subType in _structure.CoverageSubTypes)
            {
                subTypeCategoryTags.Append($"\n    <category"
                    + $"\n      code=\"{subType.CoverageSubTypeCode}\" "
                    + $"\n      typelist = \"CoverageSubtype\" />");
            }
            _coverageTypeSnippet = $"\n  <typecode " +
                $"\n    code=\"{_structure.CoverageTypeCode}\" "
                + $"\n    desc=\"{getCoverageTypeDescFromXmlFile()}\" "
                + $"\n    identifierCode=\"{_structure.CoverageTypeCode}\" "
                + $"\n    name=\"{_structure.CoverageTypeName}\">"
                + "\n    <category "
                + "\n       code=\"BCP\" "
                + "\n       typelist=\"PolicyType\"/> "
                + "\n    <category "
                + "\n       code=\"PC\" "
                + "\n       typelist=\"SourceSystem\"/> "
                + subTypeCategoryTags.ToString()
                + "\n  </typecode>";
        }

        private string getCoverageTypeDescFromXmlFile()
        {
            string coverageTypeTtxFullPath = $"{_inputPath}//{COVERAGE_TYPE_TTX_FILENAME}";
            XmlTextReader reader = new XmlTextReader(coverageTypeTtxFullPath);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("typecode") && reader.GetAttribute(0).Equals(_coverageTypeCode))
                    return reader.GetAttribute(1); // description
            }
            return "";
        }

        private void GenerateCoverageSubTypeSnippet()
        {
            foreach (var subType in _structure.CoverageSubTypes)
            {
                string exposureTypeCode;
                switch (subType.ExposureTypeCode)
                {
                    case "General": exposureTypeCode = "GeneralDamage"; break;
                    case "BodilyInjury": exposureTypeCode = "BodilyInjuryDamage"; break;
                    case "Content": exposureTypeCode = "Content"; break;
                    case "Med Pay": exposureTypeCode = "MedPay"; break;
                    //...
                    default: exposureTypeCode = $"{subType.ExposureTypeCode}**look for something similar manually**\""; break;

                }
                var subTypeTag = "   <typecode"
                + $"\n      code=\"{subType.CoverageSubTypeCode}\" "
                + $"\n      name=\"{subType.CoverageSubTypeName}\" "
                + $"\n      desc=\"{subType.CoverageSubTypeName}\">"
                + "\n       <category"
                + $"\n          code=\"{_structure.CoverageTypeCode}\" "
                + "\n          typelist=\"CoverageType\"/>"
                + "\n       <category"
                + $"\n          code=\"{exposureTypeCode}\""
                + "\n          typelist=\"ExposureType\" /> "
                + "\n   </typecode>";
                _subTypeSnippet.Append($"\n{subTypeTag}");
                GenerateExposureTypeSnippet(subType.CoverageSubTypeCode, subType.ExposureTypeCode);
                if (exposureTypeCode != "General")
                {
                    GenerateLossPartyTypeSnippet(subType.CoverageSubTypeCode, subType.ExposureTypeCode);
                }
                GenerateCostCategorySnippet(subType);
            }
        }
        private void GenerateExposureTypeSnippet(string coverageSubTypeCode, string exposureTypeCode)
        {
            var exposureTypeTag = $"\n <category      code=\"{coverageSubTypeCode}\"       typelist=\"CoverageSubtype\"/> ";
            _exposureTypeSnippet.Append(exposureTypeTag);
        }
        private void GenerateLossPartyTypeSnippet(string coverageSubTypeCode, string exposureTypeCode)
        {
            var lossPartyTypeTag = $"\n	<category code=\"{coverageSubTypeCode}\" typelist=\"CoverageSubType\"/>";
            _lossPartyTypeSnippet.Append(lossPartyTypeTag);
        }
        private void GenerateCostCategorySnippet(CoverageSubTypeStruct subType)
        { 
            var costCategoryTags = new StringBuilder();
            foreach (var costCategory in subType.CostCategories)
            {   // TODO if CostCategoryCode already exists, just add categories to it!
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
                    + $"\n       code = \"{comTermPattern.CovTermCodeFromTtx}\" "
                    + $"\n       typelist = \"CovTermPattern\"/> ");

                    _covTermPatternSnippet.Append($"\n{comTermPattern.WholeTag}");
                }
                costCategoryTags.Append("\n   </typecode>  ");
            }
            _costCategorySnippet.Append($"\n{costCategoryTags}");
        }

        private void GenerateISOMappingSnippet()
        {
            foreach(var item in _isoMappingList)
            {
                _isoMappingSnippet.AppendLine(item.AsLine());
            }
        }
        private string getCovTermPatternTagFromXml(string covTermPatternCode, string costCategoryCode, out string identifier, out string codeFromTtx)
        {   // TODO : if covTermPatternCode already exist, just add costCategoryCode tag to it!

            string covTermPatternTtxFullPath = $"{_inputPath}//{COVTERM_PATTERN_TTX_FILENAME}";
            XmlTextReader reader = new XmlTextReader(covTermPatternTtxFullPath);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("typecode") && reader.GetAttribute(2).Equals(covTermPatternCode))
                {
                    identifier = reader.GetAttribute(2);
                    codeFromTtx = reader.GetAttribute(0);
                    XmlDocument doc = new XmlDocument();
                    XmlNode node = doc.ReadNode(reader);
                    XmlNode ccCategory = doc.CreateNode(XmlNodeType.Element, "category", node.NamespaceURI); 
                    var attrCode = doc.CreateAttribute("code");
                    attrCode.Value = costCategoryCode;
                    var attrTl = doc.CreateAttribute("typelist");
                    attrTl.Value = "CostCategory";
                    ccCategory.Attributes.Append(attrCode);
                    ccCategory.Attributes.Append(attrTl);
                    node.AppendChild(ccCategory);

                    var snippet = "  " + node.OuterXml.Replace("</typecode>", "\n  </typecode>").Replace(" xmlns=\"http://guidewire.com/typelists\"", "");
                    return snippet;
                }
            }
            throw new Exception($"covterm pattern code not found : {covTermPatternCode}");
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
