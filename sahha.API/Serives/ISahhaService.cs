using sahha.API.Entities;

namespace sahha.API.Serives;

public interface ISahhaService
{
    Task<string> GetTokenAsync();
    Task<ProfileTokenResponse> RegisterProfileAsync(Guid externalId);
    Task<ProfileTokenResponse> GetProfileTokenAsync(Guid externalId);
    Task<List<BiomarkerData>> GetBiomarkersAsync(Guid externalId, string categories, string types, DateTime startDateTime, DateTime endDateTime);

}
