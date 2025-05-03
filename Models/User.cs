using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string NickName { get; set; }
    }
}