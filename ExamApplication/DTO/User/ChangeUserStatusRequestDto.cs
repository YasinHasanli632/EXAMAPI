using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    /// <summary>
    /// User-in aktiv/passiv statusunu dəyişmək üçün istifadə olunan request modelidir.
    /// </summary>
    public class ChangeUserStatusRequestDto
    {
        /// <summary>
        /// Statusu dəyişdiriləcək user-in Id-si.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User aktiv olacaqsa true, passiv olacaqsa false.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
