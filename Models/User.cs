using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string NickName { get; set; }
        public string? Name { get; set; } = null;
        public string? Surname { get; set; } = null;
        public int Solved { get; set; } = 0;
        public int Submitted { get; set; } = 0;
        //N:1
        public int? OrganizationId { get; set; } = null;
        public Organization? Organization { get; set; } = null;
        //N:1
        public int? CountryId { get; set; } = null;
        public Country? Country { get; set; } = null;
        //1:N
        public List<Submission> Submissions { get; set; } = new List<Submission>();
        //N:1
        public int? RoleId { get; set; } = null;
        public UserRole? Role { get; set; } = null;
        //1:N
        public List<Token> Tokens { get; set; } = new List<Token>();
        //
        public string EMail { get; set; } = null!;
        public string PasswordHash { get; set; } = "395022931";
        public string? PhotoPath { get; set; } = null!;
    }
}