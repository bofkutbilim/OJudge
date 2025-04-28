using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace OJudge.Models
{
    public class User
    {
        public int Id { get; set; }
        public string NickName { get; set; } = string.Empty;
    }
}