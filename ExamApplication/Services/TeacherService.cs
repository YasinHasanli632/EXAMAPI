using ExamApplication.DTO.Teacher;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Yeni teacher yaradır
        public async Task<TeacherDto> CreateAsync(CreateTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            if (request.UserId <= 0)
                throw new Exception("User məlumatı düzgün deyil");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new Exception("Müəllimin adı boş ola bilməz");

            if (string.IsNullOrWhiteSpace(request.Department))
                throw new Exception("Müəllimin bölməsi boş ola bilməz");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new Exception("User tapılmadı");

            var existingTeacher = await _unitOfWork.Teachers.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingTeacher != null)
                throw new Exception("Bu user üçün artıq müəllim profili mövcuddur");

            var teacher = new Teacher
            {
                UserId = request.UserId,
                FullName = request.FullName.Trim(),
                Department = request.Department.Trim()
            };

            await _unitOfWork.Teachers.AddAsync(teacher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = user.Username,
                Email = user.Email
            };
        }

        // Mövcud teacher-i yeniləyir
        public async Task<TeacherDto> UpdateAsync(UpdateTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            if (request.Id <= 0)
                throw new Exception("Teacher id düzgün deyil");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new Exception("Müəllimin adı boş ola bilməz");

            if (string.IsNullOrWhiteSpace(request.Department))
                throw new Exception("Müəllimin bölməsi boş ola bilməz");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.Id, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            teacher.FullName = request.FullName.Trim();
            teacher.Department = request.Department.Trim();

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var user = await _unitOfWork.Users.GetByIdAsync(teacher.UserId, cancellationToken);
            if (user == null)
                throw new Exception("Müəllimə bağlı user tapılmadı");

            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = user.Username,
                Email = user.Email
            };
        }

        // Id-yə görə teacher gətirir
        public async Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var user = await _unitOfWork.Users.GetByIdAsync(teacher.UserId, cancellationToken);
            if (user == null)
                throw new Exception("Müəllimə bağlı user tapılmadı");

            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = user.Username,
                Email = user.Email
            };
        }

        // UserId-yə görə teacher gətirir
        public async Task<TeacherDto> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var teacher = await _unitOfWork.Teachers.GetByUserIdAsync(userId, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var user = await _unitOfWork.Users.GetByIdAsync(teacher.UserId, cancellationToken);
            if (user == null)
                throw new Exception("Müəllimə bağlı user tapılmadı");

            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = user.Username,
                Email = user.Email
            };
        }

        // Id-yə görə teacher detail gətirir
        public async Task<TeacherDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var classRooms = await GetClassRoomsByTeacherIdAsync(id, cancellationToken);
            var subjects = await GetSubjectsByTeacherIdAsync(id, cancellationToken);

            return new TeacherDetailsDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = teacher.User.Username,
                Email = teacher.User.Email,
                Subjects = subjects,
                ClassRooms = classRooms,
                ExamCount = teacher.CreatedExams.Count
            };
        }

        // Bütün teacher-ləri gətirir
        public async Task<List<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(cancellationToken);

            return teachers.Select(x => new TeacherDto
            {
                Id = x.Id,
                UserId = x.UserId,
                FullName = x.FullName,
                Department = x.Department,
                UserName = x.User.Username,
                Email = x.User.Email
            }).ToList();
        }

        // Teacher-ə subject bağlayır
        public async Task AssignSubjectAsync(AssignSubjectToTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Fənn tapılmadı");

            bool exists = await _unitOfWork.TeacherSubjects.ExistsAsync(request.TeacherId, request.SubjectId, cancellationToken);
            if (exists)
                throw new Exception("Bu müəllim artıq həmin fənnə bağlıdır");

            var teacherSubject = new TeacherSubject
            {
                TeacherId = request.TeacherId,
                SubjectId = request.SubjectId
            };

            await _unitOfWork.TeacherSubjects.AddAsync(teacherSubject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-dən subject çıxarır
        public async Task RemoveSubjectAsync(RemoveSubjectFromTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            var teacherSubject = await _unitOfWork.TeacherSubjects.GetByTeacherAndSubjectAsync(
                request.TeacherId,
                request.SubjectId,
                cancellationToken);

            if (teacherSubject == null)
                throw new Exception("Müəllim-fənn əlaqəsi tapılmadı");

            _unitOfWork.TeacherSubjects.Remove(teacherSubject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-ə class bağlayır
        public async Task AssignClassRoomAsync(AssignClassRoomToTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            bool exists = await _unitOfWork.TeacherClassRooms.ExistsAsync(request.TeacherId, request.ClassRoomId, cancellationToken);
            if (exists)
                throw new Exception("Bu müəllim artıq həmin sinfə bağlıdır");

            var teacherClassRoom = new TeacherClassRoom
            {
                TeacherId = request.TeacherId,
                ClassRoomId = request.ClassRoomId
            };

            await _unitOfWork.TeacherClassRooms.AddAsync(teacherClassRoom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-dən class çıxarır
        public async Task RemoveClassRoomAsync(RemoveClassRoomFromTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz");

            var teacherClassRoom = await _unitOfWork.TeacherClassRooms.GetByTeacherAndClassRoomAsync(
                request.TeacherId,
                request.ClassRoomId,
                cancellationToken);

            if (teacherClassRoom == null)
                throw new Exception("Müəllim-sinif əlaqəsi tapılmadı");

            _unitOfWork.TeacherClassRooms.Remove(teacherClassRoom);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-in bütün subject-lərini gətirir
        public async Task<List<TeacherSubjectDto>> GetSubjectsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(teacherId, cancellationToken);

            return items.Select(x => new TeacherSubjectDto
            {
                Id = x.Id,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject.Name
            }).ToList();
        }

        // Teacher-in bütün class-larını gətirir
        public async Task<List<TeacherClassRoomDto>> GetClassRoomsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.TeacherClassRooms.GetByTeacherIdAsync(teacherId, cancellationToken);

            return items.Select(x => new TeacherClassRoomDto
            {
                Id = x.Id,
                ClassRoomId = x.ClassRoomId,
                ClassRoomName = x.ClassRoom.Name,
                Grade = x.ClassRoom.Grade
            }).ToList();
        }

        // Teacher-i silir
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(id, cancellationToken);
            if (teacherSubjects.Any())
                throw new Exception("Bu müəllimə bağlı fənlər var. Əvvəlcə əlaqələri silin");

            var teacherClassRooms = await _unitOfWork.TeacherClassRooms.GetByTeacherIdAsync(id, cancellationToken);
            if (teacherClassRooms.Any())
                throw new Exception("Bu müəllimə bağlı siniflər var. Əvvəlcə əlaqələri silin");

            if (teacher.CreatedExams.Any())
                throw new Exception("Bu müəllimə bağlı imtahanlar var. Əvvəlcə onları idarə edin");

            _unitOfWork.Teachers.Remove(teacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
