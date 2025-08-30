
using System;
using Microsoft.AspNetCore.Mvc;
using static System.Ulid;

namespace ClientService.Domain
{



public class Client
{
    public string Id { get; set; } = Ulid.NewUlid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Active { get; set; } = true;

    }

}