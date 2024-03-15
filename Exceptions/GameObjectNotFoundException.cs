using System;
using System.Management.Instrumentation;

namespace LibOnward.Exceptions;

public class GameObjectNotFoundException : Exception
{
    public GameObjectNotFoundException(string message) : base(message)
    {
    }
}