using ExamApplication.DTO.Notification;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy(
            [FromQuery] bool? isRead,
            [FromQuery] int? type,
            CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetMyNotificationsAsync(isRead, type, cancellationToken);
            return Ok(result);
        }

        [HttpGet("my/latest")]
        public async Task<IActionResult> GetMyLatest(
            [FromQuery] int take = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _notificationService.GetMyLatestNotificationsAsync(take, cancellationToken);
            return Ok(result);
        }

        [HttpGet("my/unread-count")]
        public async Task<IActionResult> GetMyUnreadCount(CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetMyUnreadCountAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _notificationService.MarkAsReadAsync(id, cancellationToken);
            return Ok(new { message = "Bildiriş oxunmuş kimi qeyd edildi." });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            await _notificationService.MarkAllAsReadAsync(cancellationToken);
            return Ok(new { message = "Bütün bildirişlər oxunmuş kimi qeyd edildi." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _notificationService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Bildiriş silindi." });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateNotificationDto request,
            CancellationToken cancellationToken)
        {
            await _notificationService.CreateAsync(request, cancellationToken);
            return Ok(new { message = "Bildiriş uğurla yaradıldı." });
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> CreateBulk(
            [FromBody] List<CreateNotificationDto> requests,
            CancellationToken cancellationToken)
        {
            await _notificationService.CreateBulkAsync(requests, cancellationToken);
            return Ok(new { message = "Bildirişlər uğurla yaradıldı." });
        }
    }
}