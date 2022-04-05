﻿using Microsoft.AspNetCore.Mvc;
using ReportsAPI.Tools;
using ReportsBLL.DataTransferObjects.Employees;
using ReportsBLL.Interfaces.Services;

namespace ReportsAPI.Controllers;

public class EmployeeController : BaseApiController
{
    private readonly IEmployeeService _employeeService;
    private string previousUsernameFilter;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? usernameFilter, int? pageNumber)
    {
        if (usernameFilter != previousUsernameFilter)
            pageNumber = 1;

        previousUsernameFilter = usernameFilter;

        var response = await _employeeService.GetAllAsync();
        var employees = response.Resource;

        if (!string.IsNullOrEmpty(usernameFilter))
            employees = employees.Where(e => e.Username == usernameFilter).ToList();

        const int pageSize = 3;
        return Ok(new PaginatedList<EmployeeViewModel>(employees.ToList(), pageNumber ?? 1, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(ulong id)
    {
        var response = await _employeeService.GetAsync(id);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource); // TODO: Change DTO -> ViewModel
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] AddEmployeeDto addEmployeeDto)
    {
        var response = await _employeeService.SaveAsync(addEmployeeDto);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(ulong id)
    {
        var success = await _employeeService.DeleteAsync(id);
        return success ? Ok(success) : BadRequest(success);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
    {
        var response = await _employeeService.UpdateAsync(id, updateEmployeeDto);
        if (!response.Success) return BadRequest(response.ErrorMessage);

        return Ok(response.Resource);
    }
}