using ExamApplication.DTO.Subject;
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
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SubjectDto> CreateAsync(CreateSubjectDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Fənn adı boş ola bilməz.");

            var normalizedName = request.Name.Trim();

            var nameExists = await _unitOfWork.Subjects.ExistsByNameAsync(normalizedName, cancellationToken);
            if (nameExists)
                throw new Exception("Bu adda subject artıq mövcuddur.");

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var normalizedCode = request.Code.Trim().ToUpperInvariant();
                var codeExists = await _unitOfWork.Subjects.ExistsByCodeAsync(normalizedCode, cancellationToken);
                if (codeExists)
                    throw new Exception("Bu kodda subject artıq mövcuddur.");
            }

            if (request.WeeklyHours < 0)
                throw new Exception("WeeklyHours mənfi ola bilməz.");

            var subject = new Subject
            {
                Name = normalizedName,
                Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim().ToUpperInvariant(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                WeeklyHours = request.WeeklyHours,
                IsActive = request.IsActive
            };

            await _unitOfWork.Subjects.AddAsync(subject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapSubjectDto(subject);
        }

        public async Task<SubjectDto> UpdateAsync(UpdateSubjectDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.Id <= 0)
                throw new Exception("Subject Id düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new Exception("Fənn adı boş ola bilməz.");

            if (request.WeeklyHours < 0)
                throw new Exception("WeeklyHours mənfi ola bilməz.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.Id, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var normalizedName = request.Name.Trim();

            var nameExists = await _unitOfWork.Subjects.ExistsByNameAsync(normalizedName, cancellationToken);
            if (nameExists && !string.Equals(subject.Name, normalizedName, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Bu adda başqa subject artıq mövcuddur.");

            string? normalizedCode = string.IsNullOrWhiteSpace(request.Code)
                ? null
                : request.Code.Trim().ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(normalizedCode))
            {
                var codeExists = await _unitOfWork.Subjects.ExistsByCodeAsync(normalizedCode, cancellationToken);
                if (codeExists && !string.Equals(subject.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Bu kodda başqa subject artıq mövcuddur.");
            }

            subject.Name = normalizedName;
            subject.Code = normalizedCode;
            subject.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            subject.WeeklyHours = request.WeeklyHours;
            subject.IsActive = request.IsActive;

            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapSubjectDto(subject);
        }

        public async Task<SubjectDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Subject Id düzgün deyil.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(id, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            return MapSubjectDto(subject);
        }

        public async Task<SubjectDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Subject Id düzgün deyil.");

            var subject = await _unitOfWork.Subjects.GetByIdWithDetailsAsync(id, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            return MapSubjectDetailsDto(subject);
        }

        public async Task<List<SubjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync(cancellationToken);

            return subjects
                .OrderBy(x => x.Name)
                .Select(MapSubjectDto)
                .ToList();
        }

        public async Task<List<SubjectDetailsDto>> GetAllDetailsAsync(CancellationToken cancellationToken = default)
        {
            var subjects = await _unitOfWork.Subjects.GetAllWithDetailsAsync(cancellationToken);

            return subjects
                .OrderBy(x => x.Name)
                .Select(MapSubjectDetailsDto)
                .ToList();
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Subject Id düzgün deyil.");

            var subject = await _unitOfWork.Subjects.GetByIdWithDetailsAsync(id, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var teachers = await _unitOfWork.TeacherSubjects.GetBySubjectIdAsync(id, cancellationToken);
            if (teachers.Any())
                throw new Exception("Bu subject müəllimlərlə bağlıdır. Əvvəlcə əlaqələri silin.");

            _unitOfWork.Subjects.Remove(subject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeStatusAsync(ChangeSubjectStatusDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.SubjectId <= 0)
                throw new Exception("SubjectId düzgün deyil.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            subject.IsActive = request.IsActive;
            _unitOfWork.Subjects.Update(subject);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SyncTeachersAsync(SyncSubjectTeachersDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.SubjectId <= 0)
                throw new Exception("SubjectId düzgün deyil.");

            request.TeacherIds ??= new List<int>();

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var normalizedTeacherIds = request.TeacherIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (normalizedTeacherIds.Count > 0)
            {
                var teachers = await _unitOfWork.Teachers.GetByIdsWithDetailsAsync(normalizedTeacherIds, cancellationToken);
                var foundTeacherIds = teachers.Select(x => x.Id).ToHashSet();

                var missingTeacherIds = normalizedTeacherIds
                    .Where(x => !foundTeacherIds.Contains(x))
                    .ToList();

                if (missingTeacherIds.Any())
                    throw new Exception($"Bəzi teacher-lər tapılmadı: {string.Join(", ", missingTeacherIds)}");
            }

            var currentItems = await _unitOfWork.TeacherSubjects.GetBySubjectIdAsync(request.SubjectId, cancellationToken);
            var currentTeacherIds = currentItems.Select(x => x.TeacherId).ToHashSet();
            var newTeacherIds = normalizedTeacherIds.ToHashSet();

            var removeItems = currentItems
                .Where(x => !newTeacherIds.Contains(x.TeacherId))
                .ToList();

            if (removeItems.Any())
                await _unitOfWork.TeacherSubjects.RemoveRangeAsync(removeItems, cancellationToken);

            var addTeacherIds = newTeacherIds
                .Where(x => !currentTeacherIds.Contains(x))
                .ToList();

            foreach (var teacherId in addTeacherIds)
            {
                await _unitOfWork.TeacherSubjects.AddAsync(new TeacherSubject
                {
                    TeacherId = teacherId,
                    SubjectId = request.SubjectId,
                    IsActive = true
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<SubjectTeacherDto>> GetTeachersBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        {
            if (subjectId <= 0)
                throw new Exception("SubjectId düzgün deyil.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var teacherSubjects = await _unitOfWork.TeacherSubjects.GetBySubjectIdAsync(subjectId, cancellationToken);

            return teacherSubjects
                .OrderBy(x => x.Teacher.FullName)
                .Select(x => new SubjectTeacherDto
                {
                    TeacherId = x.TeacherId,
                    UserId = x.Teacher.UserId,
                    FullName = x.Teacher.FullName,
                    UserName = x.Teacher.User?.Username ?? string.Empty,
                    Email = x.Teacher.User?.Email ?? string.Empty,
                    PhotoUrl = x.Teacher.User?.PhotoUrl,
                    Status = x.Teacher.Status.ToString(),
                    IsActive = x.IsActive
                })
                .ToList();
        }

        private static SubjectDto MapSubjectDto(Subject subject)
        {
            return new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Code = subject.Code,
                Description = subject.Description,
                WeeklyHours = subject.WeeklyHours,
                IsActive = subject.IsActive
            };
        }

        private static SubjectDetailsDto MapSubjectDetailsDto(Subject subject)
        {
            var teachers = subject.TeacherSubjects?
                .OrderBy(x => x.Teacher.FullName)
                .Select(x => new SubjectTeacherDto
                {
                    TeacherId = x.TeacherId,
                    UserId = x.Teacher.UserId,
                    FullName = x.Teacher.FullName,
                    UserName = x.Teacher.User?.Username ?? string.Empty,
                    Email = x.Teacher.User?.Email ?? string.Empty,
                    PhotoUrl = x.Teacher.User?.PhotoUrl,
                    Status = x.Teacher.Status.ToString(),
                    IsActive = x.IsActive
                })
                .ToList() ?? new List<SubjectTeacherDto>();

            return new SubjectDetailsDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Code = subject.Code,
                Description = subject.Description,
                WeeklyHours = subject.WeeklyHours,
                IsActive = subject.IsActive,
                TeacherCount = teachers.Count,
                Teachers = teachers
            };
        }
    }
}
