using System;

namespace GieudexPol.Domain.Auth
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string email) : base($"User with email {email} already exists.") { }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Invalid credentials.") { }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email) : base($"User with email {email} not found.") { }
    }
}