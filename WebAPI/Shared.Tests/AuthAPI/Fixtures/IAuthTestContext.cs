using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.AuthAPI.Fixtures;

public interface IAuthTestContext : IAsyncLifetime
{
    HttpClient Client { get; }
    AuthWebApplicationFactory Factory { get; }
}

