﻿using System;
using TtxGenerator.net;

const string INPUT_FILES_PATH = "C:\\dev\\bmic\\TtxGenerator.net"; // The path where input files reside.(xlsx and 2 ttx's)
const string TARGET_COVERAGETYPE = "GL1FeeBasedCounselingCov_Ext"; 
LobProfile LOB_INPUT_PROFILE = LobProfile.LIABILITY; // LIABILITY or PROPERTY coverage type ?

/*
 App may generate multiple covTermPattern tags for same covTermPattern code. In that case GW Studio will scream error, 
so you can easily merge the two (or more) tags manually.
 */

XlsxReader reader = new XlsxReader();
reader.FilterByCoverageType(INPUT_FILES_PATH, TARGET_COVERAGETYPE, LOB_INPUT_PROFILE);
if(reader.RowList.Count == 0)
{
    Console.WriteLine($"Could not find coverage type code {TARGET_COVERAGETYPE} in spreadsheet!");
    Console.ReadLine();
    Environment.Exit(1);
}
SnippetGenerator snippetGenerator = new SnippetGenerator(reader.RowList, TARGET_COVERAGETYPE, INPUT_FILES_PATH);

Console.WriteLine(snippetGenerator.PolicyTypeSnippet);

snippetGenerator.GenerateCoverageTypeSnippet();
Console.WriteLine(snippetGenerator.CoverageTypeSnippet);

snippetGenerator.GenerateCoverageSubTypeSnippet();
Console.WriteLine(snippetGenerator.CoverageSubtypeSnippet);
Console.WriteLine(snippetGenerator.ExposureTypeSnippet);
Console.WriteLine(snippetGenerator.CostCategorySnippet);
Console.WriteLine(snippetGenerator.CovTermPatternSnippet);

Console.ReadLine();