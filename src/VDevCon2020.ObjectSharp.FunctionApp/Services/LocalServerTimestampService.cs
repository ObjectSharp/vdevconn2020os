using System;

namespace VDevCon2020.ObjectSharp.FunctionApp.Services
{
    public class LocalServerTimestampService: ITimestampService
    {
        public DateTimeOffset Now() => DateTimeOffset.Now;
    }
}
