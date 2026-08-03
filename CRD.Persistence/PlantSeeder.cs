using ClosedXML.Excel;
using CRD.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CRD.Persistence
{
    public class PlantSeeder
    {
        private readonly ILogger<PlantSeeder> logger;

        public PlantSeeder(ILogger<PlantSeeder> logger)
        {
            this.logger = logger;
        }

        public async Task SeedPlantsFromExcelAsync(ApplicationDbContext context, string languagesPath, string cropsPath, string treesPath)
        {
            try
            {
                logger.LogInformation("Starting plant seeding from Excel files");

                // First, ensure languages are seeded and get the mapping
                var languageMapping = await SeedLanguagesAsync(context, ReadLanguages(languagesPath));

                // Seed crops
                await SeedCropsAsync(context, cropsPath, languageMapping);

                // Seed trees
                await SeedTreesAsync(context, treesPath, languageMapping);

                logger.LogInformation("Plant seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during plant seeding");
                throw;
            }
        }

        private List<(int ExcelId, string Name)> ReadLanguages(string languagesPath)
        {
            var languages = new List<(int ExcelId, string Name)>();
            
            using (var wb = new XLWorkbook(languagesPath))
            {
                var sheet = wb.Worksheet(1);
                var usedRange = sheet.RangeUsed();
                
                if (usedRange == null)
                    return languages;

                // Skip header row (row 1)
                var rowCount = usedRange.LastRow().RowNumber();
                
                for (int i = 2; i <= rowCount; i++)
                {
                    var langName = sheet.Cell(i, 1).Value.ToString();
                    if (!string.IsNullOrWhiteSpace(langName))
                    {
                        languages.Add((i - 1, langName)); // ExcelId = row number - 1 (since row 2 = ID 1)
                    }
                }
            }

            logger.LogInformation("Loaded {count} languages from Excel", languages.Count);
            return languages;
        }

        private async Task<Dictionary<int, int>> SeedLanguagesAsync(ApplicationDbContext context, List<(int ExcelId, string Name)> languages)
        {
            logger.LogInformation("Seeding {count} languages", languages.Count);
            
            // Clear all translations first (they reference languages via foreign key)
            var existingTranslations = await context.Set<Translation>().ToListAsync();
            if (existingTranslations.Any())
            {
                logger.LogInformation("Clearing {count} existing translations", existingTranslations.Count);
                context.Set<Translation>().RemoveRange(existingTranslations);
                await context.SaveChangesAsync();
            }
            
            // Clear all crops and trees (they have translations)
            var existingCrops = await context.Set<Crop>().ToListAsync();
            var existingTrees = await context.Set<Tree>().ToListAsync();
            if (existingCrops.Any())
            {
                logger.LogInformation("Clearing {count} existing crops", existingCrops.Count);
                context.Set<Crop>().RemoveRange(existingCrops);
                await context.SaveChangesAsync();
            }
            if (existingTrees.Any())
            {
                logger.LogInformation("Clearing {count} existing trees", existingTrees.Count);
                context.Set<Tree>().RemoveRange(existingTrees);
                await context.SaveChangesAsync();
            }
            
            // Clear all existing languages to ensure we're using only the new data
            var existingLanguages = await context.Set<Languange>().ToListAsync();
            if (existingLanguages.Any())
            {
                logger.LogInformation("Clearing {count} existing languages", existingLanguages.Count);
                context.Set<Languange>().RemoveRange(existingLanguages);
                await context.SaveChangesAsync();
            }
            
            // Reset the identity seed for Languange table to start from 1
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Languanges', RESEED, 0)");
            logger.LogInformation("Reset Languange table identity seed to 0");
            
            var mapping = new Dictionary<int, int>(); // ExcelId -> DatabaseId
            
            foreach (var (excelId, name) in languages)
            {
                var lang = new Languange { Name = name };
                context.Set<Languange>().Add(lang);
                await context.SaveChangesAsync();
                mapping[excelId] = lang.Id;
                logger.LogInformation("Created language {name} with ID {id}", name, lang.Id);
            }

            logger.LogInformation("Languages seeding completed. Mapped {count} languages", mapping.Count);
            return mapping;
        }

        private async Task SeedCropsAsync(ApplicationDbContext context, string cropsPath, Dictionary<int, int> languageMapping)
        {
            using (var wb = new XLWorkbook(cropsPath))
            {
                var sheet = wb.Worksheet(1);
                var usedRange = sheet.RangeUsed();
                
                if (usedRange == null)
                    return;

                var rowCount = usedRange.LastRow().RowNumber();
                
                // Parse header row to get language IDs from Excel (column headers)
                var headerRow = 1;
                var languageIdMapping = new Dictionary<int, int>(); // column index -> actual database language ID
                
                var colCount = usedRange.LastColumn().ColumnNumber();
                for (int col = 5; col <= colCount; col++)
                {
                    var headerValue = sheet.Cell(headerRow, col).Value.ToString();
                    if (int.TryParse(headerValue, out int excelLangId))
                    {
                        if (languageMapping.TryGetValue(excelLangId, out int dbLangId))
                        {
                            languageIdMapping[col] = dbLangId;
                        }
                    }
                }

                // Process crop rows (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    var commonName = sheet.Cell(row, 1).Value.ToString();
                    var type = sheet.Cell(row, 2).Value.ToString();
                    var aez = sheet.Cell(row, 3).Value.ToString();
                    var botanicalName = sheet.Cell(row, 4).Value.ToString();

                    if (string.IsNullOrWhiteSpace(commonName) || string.IsNullOrWhiteSpace(botanicalName))
                        continue;

                    var crop = new Crop
                    {
                        CommonName = commonName,
                        BotanicalName = botanicalName,
                        PlantType = "Crop",
                        CropType = type,
                        Aez = aez,
                        Translations = new List<Translation>()
                    };

                    // Add translations
                    foreach (var (colIndex, langId) in languageIdMapping)
                    {
                        var translatedValue = sheet.Cell(row, colIndex).Value.ToString();
                        if (!string.IsNullOrWhiteSpace(translatedValue))
                        {
                            crop.Translations.Add(new Translation
                            {
                                LanguageId = langId,
                                Translated = translatedValue
                            });
                        }
                    }

                    context.Set<Crop>().Add(crop);
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Crops seeded");
            }
        }

        private async Task SeedTreesAsync(ApplicationDbContext context, string treesPath, Dictionary<int, int> languageMapping)
        {
            using (var wb = new XLWorkbook(treesPath))
            {
                var sheet = wb.Worksheet(1);
                var usedRange = sheet.RangeUsed();
                
                if (usedRange == null)
                    return;

                var rowCount = usedRange.LastRow().RowNumber();
                
                // Parse header row to get language IDs from Excel (column headers)
                var headerRow = 1;
                var languageIdMapping = new Dictionary<int, int>(); // column index -> actual database language ID
                
                var colCount = usedRange.LastColumn().ColumnNumber();
                for (int col = 4; col <= colCount; col++)
                {
                    var headerValue = sheet.Cell(headerRow, col).Value.ToString();
                    if (int.TryParse(headerValue, out int excelLangId))
                    {
                        if (languageMapping.TryGetValue(excelLangId, out int dbLangId))
                        {
                            languageIdMapping[col] = dbLangId;
                        }
                    }
                }

                // Process tree rows (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    var commonName = sheet.Cell(row, 1).Value.ToString();
                    var botanicalName = sheet.Cell(row, 2).Value.ToString();
                    var category = sheet.Cell(row, 3).Value.ToString();

                    if (string.IsNullOrWhiteSpace(botanicalName))
                        continue;

                    var tree = new Tree
                    {
                        CommonName = commonName,
                        BotanicalName = botanicalName,
                        PlantType = category,
                        Info = category,
                        Translations = new List<Translation>(),
                        Othernames = new List<string>()
                    };

                    // Add translations
                    foreach (var (colIndex, langId) in languageIdMapping)
                    {
                        var translatedValue = sheet.Cell(row, colIndex).Value.ToString();
                        if (!string.IsNullOrWhiteSpace(translatedValue))
                        {
                            tree.Translations.Add(new Translation
                            {
                                LanguageId = langId,
                                Translated = translatedValue
                            });
                        }
                    }

                    context.Set<Tree>().Add(tree);
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Trees seeded");
            }
        }
    }
}
