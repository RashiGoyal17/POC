//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;

class Program
{
    static void Main()
    {
        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine($"\nBCrypt hash:\n{hash}");
    }
}
