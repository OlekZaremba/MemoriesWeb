using MemoriesBack.Data;
using MemoriesBack.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemoriesBack.Controllers;

[ApiController]
[Route("api/group-members")]
public class GroupMemberController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupMemberController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("teacher/{teacherId}/group/{groupId}")]
    public async Task<IActionResult> GetGroupMemberByTeacherAndGroup(int teacherId, int groupId)
    {
        var member = await _context.GroupMembers
            .Where(gm => gm.UserId == teacherId && gm.UserGroupId == groupId)
            .Select(gm => new { id = gm.Id })
            .FirstOrDefaultAsync();

        if (member == null)
            return NotFound();

        return Ok(member);
    }
    
    [HttpPost("{groupMemberId}/class/{classId}")]
    public async Task<IActionResult> AssignSubjectToGroupMember(int groupMemberId, int classId)
    {
        var exists = await _context.GroupMemberClasses
            .AnyAsync(x => x.GroupMemberId == groupMemberId && x.SchoolClassId == classId);

        if (exists)
            return BadRequest("Już przypisano ten przedmiot.");

        var assignment = new GroupMemberClass
        {
            GroupMemberId = groupMemberId,
            SchoolClassId = classId
        };

        _context.GroupMemberClasses.Add(assignment);
        await _context.SaveChangesAsync();

        return Ok();
    }

}
