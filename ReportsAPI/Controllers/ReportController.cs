﻿using Microsoft.AspNetCore.Mvc;
using ReportsBLL.DataTransferObjects.Reports;
using ReportsBLL.Interfaces.Services;
using ReportsBLL.Services;

namespace ReportsAPI.Controllers;

public class ReportController : BaseApiController
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(ulong id)
    {
        var response = await _reportService.GetAsync(id);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] AddReportDto addReportDto)
    {
        var response = await _reportService.SaveAsync(addReportDto);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(ulong id, string newDescription)
    {
        var response = await _reportService.UpdateAsync(id, newDescription);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }

    [HttpPatch("/complete/{id}")]
    public async Task<IActionResult> Complete(ulong id)
    {
        var response = await _reportService.CompleteAsync(id);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }

    [HttpGet("/getAllFromSubordinates/{supervisorId}")]
    public async Task<IActionResult> GetFromSubordinates(ulong supervisorId)
    {
        var response = await _reportService.GetAllFromSubordinatesAsync(supervisorId);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }
}