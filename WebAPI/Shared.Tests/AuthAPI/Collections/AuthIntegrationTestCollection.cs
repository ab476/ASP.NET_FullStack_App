using Shared.Tests.AuthAPI.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.AuthAPI.Collections;

[CollectionDefinition(TestConstants.Auth)]
public class AuthIntegrationTestCollection
    : ICollectionFixture<AuthTestContextFixture>
{
}