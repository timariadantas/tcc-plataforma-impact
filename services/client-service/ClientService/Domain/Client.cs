
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
    public DateTime CreatedAt { get; set; } //// preenchido pelo banco
    public DateTime UpdatedAt { get; set; } // preenchido pelo banco
    public bool Active { get; set; } = true; //// default true no banco

    }

}