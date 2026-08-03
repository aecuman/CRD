using ClosedXML.Excel;
using System;
using System.IO;

// Quick test to understand Excel structure
var languagesPath = @"C:\Users\dorot\Github\CRD\sample\languages.xlsx";
var cropsPath = @"C:\Users\dorot\Github\CRD\sample\CropDataCSV.xlsx";
var treesPath = @"C:\Users\dorot\Github\CRD\sample\TreeDataCSVTest.xlsx";

Console.WriteLine("=== LANGUAGES ===");
using (var wb = new XLWorkbook(languagesPath))
{
    var sheet = wb.Worksheet(1);
    var rowCount = sheet.LastRowUsed()?.RowNumber ?? 0;
    var colCount = sheet.LastColumnUsed()?.ColumnNumber ?? 0;
    
    Console.WriteLine($"Rows: {rowCount}, Columns: {colCount}");
    Console.WriteLine("\nFirst 5 rows:");
    for (int i = 1; i <= Math.Min(5, rowCount); i++)
    {
        var values = new List<string>();
        for (int j = 1; j <= colCount; j++)
        {
            values.Add(sheet.Cell(i, j).Value.ToString());
        }
        Console.WriteLine(string.Join(" | ", values));
    }
}

Console.WriteLine("\n=== CROPS ===");
using (var wb = new XLWorkbook(cropsPath))
{
    var sheet = wb.Worksheet(1);
    var rowCount = sheet.LastRowUsed()?.RowNumber ?? 0;
    var colCount = sheet.LastColumnUsed()?.ColumnNumber ?? 0;
    
    Console.WriteLine($"Rows: {rowCount}, Columns: {colCount}");
    Console.WriteLine("\nFirst 5 rows:");
    for (int i = 1; i <= Math.Min(5, rowCount); i++)
    {
        var values = new List<string>();
        for (int j = 1; j <= Math.Min(5, colCount); j++)
        {
            values.Add(sheet.Cell(i, j).Value.ToString());
        }
        Console.WriteLine(string.Join(" | ", values));
    }
}

Console.WriteLine("\n=== TREES ===");
using (var wb = new XLWorkbook(treesPath))
{
    var sheet = wb.Worksheet(1);
    var rowCount = sheet.LastRowUsed()?.RowNumber ?? 0;
    var colCount = sheet.LastColumnUsed()?.ColumnNumber ?? 0;
    
    Console.WriteLine($"Rows: {rowCount}, Columns: {colCount}");
    Console.WriteLine("\nFirst 5 rows:");
    for (int i = 1; i <= Math.Min(5, rowCount); i++)
    {
        var values = new List<string>();
        for (int j = 1; j <= Math.Min(5, colCount); j++)
        {
            values.Add(sheet.Cell(i, j).Value.ToString());
        }
        Console.WriteLine(string.Join(" | ", values));
    }
}
