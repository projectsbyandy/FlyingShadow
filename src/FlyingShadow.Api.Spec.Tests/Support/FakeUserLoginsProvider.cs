using Ardalis.GuardClauses;
using FlyingShadow.Api.Utils;
using FlyingShadow.Client.Models;
using FlyingShadow.Core.DTO.Configuration;

namespace FlyingShadow.Api.Spec.Tests.Support;

public sealed class FakeUserLoginsProvider
{
    private readonly FakeUsers _users;

    public FakeUserLoginsProvider()
    {
        _users = Guard.Against.Null(ConfigReader.GetConfigurationSection<FakeUsers>("FakeUsers"));
    }

    public LoginDetails ValidUser()
    {
        var first = Guard.Against.Null(_users.LoginDetailsList).First();
        return new LoginDetails { Email = first.Email, Password = first.Password };
    }
}