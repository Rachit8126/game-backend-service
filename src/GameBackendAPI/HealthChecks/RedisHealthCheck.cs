using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace GameBackendAPI.HealthChecks
{
    public class RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await connectionMultiplexer.GetDatabase().PingAsync();
                return HealthCheckResult.Healthy();
            }
            catch (Exception e)
            {
                return HealthCheckResult.Degraded(e.Message);
            }
        }
    }
}
