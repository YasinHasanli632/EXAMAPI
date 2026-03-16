using ExamApplication.DTO.Class;
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
    public class ClassRoomService : IClassRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Yeni sinif yaradır
        public async Task<ClassRoomDto> CreateAsync(CreateClassRoomDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Sinif adı boş ola bilməz");

            if (request.Grade <= 0)
                throw new Exception("Sinif səviyyəsi düzgün deyil");

            bool exists = await _unitOfWork.ClassRooms.ExistsByNameAsync(request.Name.Trim(), cancellationToken);
            if (exists)
                throw new Exception("Bu adda sinif artıq mövcuddur");

            var classRoom = new ClassRoom
            {
                Name = request.Name.Trim(),
                Grade = request.Grade
            };

            await _unitOfWork.ClassRooms.AddAsync(classRoom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ClassRoomDto
            {
                Id = classRoom.Id,
                Name = classRoom.Name,
                Grade = classRoom.Grade
            };
        }

        // Mövcud sinifi yeniləyir
        public async Task<ClassRoomDto> UpdateAsync(UpdateClassRoomDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Sinif adı boş ola bilməz");

            if (request.Grade <= 0)
                throw new Exception("Sinif səviyyəsi düzgün deyil");

            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.Id, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            bool exists = await _unitOfWork.ClassRooms.ExistsByNameAsync(request.Name.Trim(), cancellationToken);
            if (exists && !string.Equals(classRoom.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Exception("Bu adda başqa sinif artıq mövcuddur");

            classRoom.Name = request.Name.Trim();
            classRoom.Grade = request.Grade;

            _unitOfWork.ClassRooms.Update(classRoom);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ClassRoomDto
            {
                Id = classRoom.Id,
                Name = classRoom.Name,
                Grade = classRoom.Grade
            };
        }

        // Id-yə görə sinifi gətirir
        public async Task<ClassRoomDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(id, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            return new ClassRoomDto
            {
                Id = classRoom.Id,
                Name = classRoom.Name,
                Grade = classRoom.Grade
            };
        }

        // Bütün sinifləri qaytarır
        public async Task<List<ClassRoomDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var classRooms = await _unitOfWork.ClassRooms.GetAllAsync(cancellationToken);

            return classRooms.Select(x => new ClassRoomDto
            {
                Id = x.Id,
                Name = x.Name,
                Grade = x.Grade
            }).ToList();
        }

        // Id-yə görə sinifi bütün detalları ilə birlikdə gətirir
        public async Task<ClassRoomDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(id, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            var students = await GetStudentsByClassRoomIdAsync(id, cancellationToken);
            var teachers = await GetTeachersByClassRoomIdAsync(id, cancellationToken);

            return new ClassRoomDetailsDto
            {
                Id = classRoom.Id,
                Name = classRoom.Name,
                Grade = classRoom.Grade,
                StudentCount = students.Count,
                TeacherCount = teachers.Count,
                Students = students,
                Teachers = teachers
            };
        }

        // Tələbəyə aid bütün sinifləri qaytarır
        public async Task<List<ClassRoomDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.StudentClasses.GetByStudentIdAsync(studentId, cancellationToken);

            return items.Select(x => new ClassRoomDto
            {
                Id = x.ClassRoom.Id,
                Name = x.ClassRoom.Name,
                Grade = x.ClassRoom.Grade
            }).ToList();
        }

        // Müəllimə aid bütün sinifləri qaytarır
        public async Task<List<ClassRoomDto>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.TeacherClassRooms.GetByTeacherIdAsync(teacherId, cancellationToken);

            return items.Select(x => new ClassRoomDto
            {
                Id = x.ClassRoom.Id,
                Name = x.ClassRoom.Name,
                Grade = x.ClassRoom.Grade
            }).ToList();
        }

        // Sinfə tələbə təyin edir
        public async Task AssignStudentAsync(AssignStudentToClassRoomDto request, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId, cancellationToken);
            if (student == null)
                throw new Exception("Tələbə tapılmadı");

            bool exists = await _unitOfWork.StudentClasses.ExistsAsync(request.StudentId, request.ClassRoomId, cancellationToken);
            if (exists)
                throw new Exception("Tələbə artıq bu sinfə bağlıdır");

            var studentClass = new StudentClass
            {
                StudentId = request.StudentId,
                ClassRoomId = request.ClassRoomId
            };

            await _unitOfWork.StudentClasses.AddAsync(studentClass, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Sinifdən tələbəni çıxarır
        public async Task RemoveStudentAsync(RemoveStudentFromClassRoomDto request, CancellationToken cancellationToken = default)
        {
            var studentClass = await _unitOfWork.StudentClasses.GetByStudentAndClassAsync(
                request.StudentId,
                request.ClassRoomId,
                cancellationToken);

            if (studentClass == null)
                throw new Exception("Student-sinif əlaqəsi tapılmadı");

            _unitOfWork.StudentClasses.Remove(studentClass);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Sinfə müəllim təyin edir
        public async Task AssignTeacherAsync(AssignTeacherToClassRoomDto request, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Müəllim tapılmadı");

            var existingTeachers = await _unitOfWork.TeacherClassRooms.GetByClassRoomIdAsync(request.ClassRoomId, cancellationToken);
            bool exists = existingTeachers.Any(x => x.TeacherId == request.TeacherId);

            if (exists)
                throw new Exception("Müəllim artıq bu sinfə bağlıdır");

            var teacherClassRoom = new TeacherClassRoom
            {
                TeacherId = request.TeacherId,
                ClassRoomId = request.ClassRoomId
            };

            await _unitOfWork.TeacherClassRooms.AddAsync(teacherClassRoom, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Sinifdən müəllimi çıxarır
        public async Task RemoveTeacherAsync(RemoveTeacherFromClassRoomDto request, CancellationToken cancellationToken = default)
        {
            var teachers = await _unitOfWork.TeacherClassRooms.GetByClassRoomIdAsync(request.ClassRoomId, cancellationToken);

            var teacherClassRoom = teachers.FirstOrDefault(x => x.TeacherId == request.TeacherId);
            if (teacherClassRoom == null)
                throw new Exception("Müəllim-sinif əlaqəsi tapılmadı");

            _unitOfWork.TeacherClassRooms.Remove(teacherClassRoom);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Sinifdə olan bütün tələbələri qaytarır
        public async Task<List<ClassRoomStudentDto>> GetStudentsByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.StudentClasses.GetByClassRoomIdAsync(classRoomId, cancellationToken);

            return items.Select(x => new ClassRoomStudentDto
            {
                StudentId = x.Student.Id,
                UserId = x.Student.UserId,
                FullName = x.Student.FullName,
                UserName = x.Student.User.Username,
                Email = x.Student.User.Email
            }).ToList();
        }

        // Sinifə bağlı bütün müəllimləri qaytarır
        public async Task<List<ClassRoomTeacherDto>> GetTeachersByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            var items = await _unitOfWork.TeacherClassRooms.GetByClassRoomIdAsync(classRoomId, cancellationToken);

            return items.Select(x => new ClassRoomTeacherDto
            {
                TeacherId = x.Teacher.Id,
                UserId = x.Teacher.UserId,
                FullName = x.Teacher.FullName,
                UserName = x.Teacher.User.Username,
                Email = x.Teacher.User.Email
            }).ToList();
        }

        // Sinifi silir
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(id, cancellationToken);
            if (classRoom == null)
                throw new Exception("Sinif tapılmadı");

            var students = await _unitOfWork.StudentClasses.GetByClassRoomIdAsync(id, cancellationToken);
            if (students.Any())
                throw new Exception("Bu sinifə bağlı tələbələr var. Əvvəlcə əlaqələri silin");

            var teachers = await _unitOfWork.TeacherClassRooms.GetByClassRoomIdAsync(id, cancellationToken);
            if (teachers.Any())
                throw new Exception("Bu sinifə bağlı müəllimlər var. Əvvəlcə əlaqələri silin");

            _unitOfWork.ClassRooms.Remove(classRoom);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
