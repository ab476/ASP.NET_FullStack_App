using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.AuthAPI.Tests;
/// <summary>
/// Identifies a test class as part of the AuthAPI integration test suite,
/// automatically binding it to the shared Auth Integration Test collection.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AuthApiIntegrationTestAttribute : CollectionAttribute
{
    public AuthApiIntegrationTestAttribute()
        : base("AuthAPI Integration Tests")
    {
    }
}
