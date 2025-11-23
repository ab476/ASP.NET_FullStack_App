namespace Shared.Tests.AuthAPITests.Collections;

[CollectionDefinition(TestConstants.Auth)]
public class AuthIntegrationTestCollection
    : ICollectionFixture<AuthTestContextFixture>
{
}