using CRD.Application.Common;
using CRD.Application.Rates.Queries;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CRD.Persistence.Mongo
{
    public class PublishedRateMongoService : IPublishedRateMongoService
    {
        private readonly IMongoCollection<ModerationReportViewModel> _collection;

        public PublishedRateMongoService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var db = client.GetDatabase(settings.Value.DatabaseName);
            _collection = db.GetCollection<ModerationReportViewModel>(settings.Value.CollectionName ?? "PublishedRateDocuments");
        }

        public async Task SaveAsync(ModerationReportViewModel report, CancellationToken cancellationToken = default)
        {
            report.PublishedOn = DateTime.UtcNow;
            report.ValidFrom = new DateTime(report.Year, 1, 1);
            report.ValidTo = new DateTime(report.Year + 1, 1, 1);

            await _collection.InsertOneAsync(report, cancellationToken: cancellationToken);
        }

        public async Task<List<ModerationReportViewModel>> QueryByDistrict(int districtRateId)
        {
            return await _collection.Find(r => r.DistrictRateId == districtRateId).ToListAsync();
        }

        public async Task<List<ModerationReportViewModel>> GetLatestByDistrictAsync(CancellationToken cancellationToken)
        {
            return await _collection.AsQueryable()
                  .GroupBy(r => r.DistrictId)
                  .Select(g => g.OrderByDescending(r => r.ValidFrom).First())
                  .ToListAsync(cancellationToken);
        }

        /* public async Task<List<ModerationReportViewModel>> QueryByGeo(BoundingBox box)
         {
             var filter = Builders<ModerationReportViewModel>.Filter.GeoWithinBox(
                 "location",
                 box.SouthWest.Lng,
                 box.SouthWest.Lat,
                 box.NorthEast.Lng,
                 box.NorthEast.Lat
             );

             return await _collection.Find(filter).ToListAsync();
         }*/
    }


}
