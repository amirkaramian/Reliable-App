using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Settings;

public class IpRateLimitSettings : IAppSettings
{
    public bool EnableEndpointRateLimiting { get; set; }
}
