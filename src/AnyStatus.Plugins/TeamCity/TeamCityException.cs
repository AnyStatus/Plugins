using System;

namespace AnyStatus
{
    public class TeamCityException : Exception
    {
        public TeamCityException(string message) : base(message)
        {
        }
    }
}
