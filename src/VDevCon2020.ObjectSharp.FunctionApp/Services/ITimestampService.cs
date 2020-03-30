using System;

namespace VDevCon2020.ObjectSharp.FunctionApp.Services
{
    public interface ITimestampService
    {
        DateTimeOffset Now();
    }
}
