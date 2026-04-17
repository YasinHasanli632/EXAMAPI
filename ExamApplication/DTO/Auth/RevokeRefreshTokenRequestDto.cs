using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Auth
{
    // YENI
    public class RevokeRefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
