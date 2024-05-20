﻿namespace KurumsalHastane.Infrastructure.PasswordHasher
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}