// See https://aka.ms/new-console-template for more information
using TtxGenerator.net;

string FILTER_COVERAGETYPE = "GL1ClergyCounselingCov_Ext"; // cmd parameter!
XlsxReader reader = new XlsxReader();
reader.FilterByCoverageType(FILTER_COVERAGETYPE);
SnippetGenerator snippetGenerator = new SnippetGenerator(reader.RowList, FILTER_COVERAGETYPE);
Console.WriteLine(snippetGenerator.PolicyTypeSnippet);
Console.WriteLine(snippetGenerator.CoverageTypeSnippet);

snippetGenerator.GenerateCoverageSubTypeSnippet();
Console.WriteLine(snippetGenerator.CoverageSubtypeSnippet);
Console.WriteLine(snippetGenerator.ExposureTypeSnippet);
Console.WriteLine(snippetGenerator.CostCategorySnippet);
Console.WriteLine(snippetGenerator.CovTermPatternSnippet);
Console.ReadLine();