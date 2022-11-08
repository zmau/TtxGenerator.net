using System;
using TtxGenerator.net;

const string INPUT_FILES_PATH = "C:\\dev\\bmic"; // The path where input files reside.(xlsx and 2 ttx's)
const string TARGET_COVERAGETYPE = "GL1CollegeStudentMed"; 
LobProfile LOB_INPUT_PROFILE = LobProfile.LIABILITY; // LIABILITY or PROPERTY coverage type ?

/*
 App may generate multiple covTermPattern tags for same covTermPattern code. In that case GW Studio will scream error, 
so you can easily merge the two (or more) tags manually.
 */

XlsxReader reader = new XlsxReader(INPUT_FILES_PATH, LOB_INPUT_PROFILE, TARGET_COVERAGETYPE);
reader.FilterByCoverageType();

SnippetGenerator snippetGenerator = new SnippetGenerator(reader.LobMappingRowList, reader.ISOMappingList, TARGET_COVERAGETYPE, INPUT_FILES_PATH);
snippetGenerator.GenerateAndWriteAll();
