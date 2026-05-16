namespace GieudexPol.Domain.Auth
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string HashedPassword { get; private set; }

        private User() { }

        public User(Guid id, string email, string hashedPassword)
        {
            Id = id;
            Email = email;
            HashedPassword = hashedPassword;
        }

        public void UpdateEmail(string newEmail)
        {
            Email = newEmail;
        }

        public void UpdatePassword(string newHashedPassword)
        {
            HashedPassword = newHashedPassword;
        }
    }
}