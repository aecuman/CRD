using CRD.Application.Rates.Queries;

namespace CRD.Application.Common
{
    public interface IPublishedRateMongoService
    {
        Task SaveAsync(ModerationReportViewModel report, CancellationToken cancellationToken = default);
        Task<List<ModerationReportViewModel>> QueryByDistrict(int districtId);
        Task<List<ModerationReportViewModel>> GetLatestByDistrictAsync(CancellationToken cancellationToken);

        // Task<List<ModerationReportViewModel>> QueryByGeo(BoundingBox box);
    }

}
