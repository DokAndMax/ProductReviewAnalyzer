using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ProductReviewAnalyzer.WebApp.Services;

public class UserSession(ProtectedSessionStorage storage)
{
    private readonly ProtectedSessionStorage storage = storage;
    private const string Key = "UserId";
    private Guid? userId;

    public async Task<Guid> GetUserIdAsync()
    {
        if (userId.HasValue)
            return userId.Value;
        ProtectedBrowserStorageResult<Guid> result = default;
        try
        {
            result = await storage.GetAsync<Guid>(Key);
        }
        catch (Exception e)
        {
            // Ignore exceptions, we'll create a new user ID
        }
        if (result.Success && result.Value != Guid.Empty)
        {
            userId = result.Value;
        }
        else
        {
            userId = Guid.NewGuid();
            await storage.SetAsync(Key, userId.Value);
        }

        return userId.Value;
    }
}