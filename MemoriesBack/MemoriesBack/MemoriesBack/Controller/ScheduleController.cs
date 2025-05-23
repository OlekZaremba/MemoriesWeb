using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MemoriesBack.DTO;
using MemoriesBack.Service;
using MemoriesBack.DTOs;

namespace MemoriesBack.Controller
{
    [ApiController]
    [Route("api/schedules")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleResponseDTO>> CreateLesson([FromBody] ScheduleRequestDTO dto)
        {
            var result = await _scheduleService.CreateLessonAsync(dto);
            return Ok(result);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> GetScheduleForGroup(
            int groupId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var result = await _scheduleService.GetScheduleForGroupAsync(groupId, from, to);
            return Ok(result);
        }

        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<List<ScheduleResponseDTO>>> GetScheduleForTeacher(
            int teacherId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var result = await _scheduleService.GetScheduleForTeacherAsync(teacherId, from, to);
            return Ok(result);
        }
    }
}
