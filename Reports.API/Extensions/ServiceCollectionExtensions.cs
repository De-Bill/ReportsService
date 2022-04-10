﻿using Microsoft.EntityFrameworkCore;
using Reports.DAL.Data;
using Reports.DAL.Data.Repositories;
using Reports.Domain.DataTransferObjects;
using Reports.Domain.Interfaces;
using Reports.Domain.Interfaces.Services;
using Reports.Domain.Models.Employees;
using Reports.Domain.Services;

namespace Reports.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<ReportsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddDatabaseDeveloperPageExceptionFilter();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services.AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IRepository<Employee>, EmployeeRepository>()
            .AddScoped<IEmployeeService,EmployeeService>()
            .AddScoped<IProblemService, ProblemService>()
            .AddScoped<IReportService, ReportService>();

    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services) =>
        services.AddAutoMapper(typeof(EmployeeViewModel))
            .AddAutoMapper(typeof(ProblemViewModel))
            .AddAutoMapper(typeof(CommentViewModel))
            .AddAutoMapper(typeof(ReportViewModel));
}