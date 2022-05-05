﻿using Reports.Domain.Interfaces;
using Reports.Domain.Models.Problems;
using Reports.Domain.Models.Reports;
using Reports.Domain.Tools.Exceptions;

namespace Reports.Domain.Models.Employees;

public class Employee : BaseEntity, IAggregateRoot, IEmployee, ISubordinate, ISupervisor
{
    private readonly List<Problem> _problems = new();
    private readonly List<ISubordinate> _subordinates = new();
    private string _username;
    protected ISupervisor? _supervisor;

    protected Employee()
    {
    }

    protected Employee(string username, ulong? supervisorId)
    {
        Username = username;
        SupervisorId = supervisorId;
    }

    public Employee(string username, ISupervisor? supervisor)
    {
        Username = username;
        _supervisor = supervisor;
        SupervisorId = supervisor?.Id;
    }

    public string Username
    {
        get => _username;
        private set
        {
            if (string.IsNullOrEmpty(value))
                throw new DomainException("Employee's username can't be null or empty");
            _username = value;
        }
    }

    public virtual ISupervisor? Supervisor
    {
        get => _supervisor;
        set
        {
            
            _supervisor = value;
            SupervisorId = value?.Id;
        }
    }

    public ulong? SupervisorId { get; private set; }

    public IEnumerable<Problem> Problems => _problems;

    public Report? Report { get; private set; }

    public IEnumerable<ISubordinate> Subordinates => _subordinates;

    public void Update(Employee updatedEmployee)
    {
        if (updatedEmployee is null)
            throw new DomainException(
                "Updated employee can't be null!",
                new ArgumentNullException(nameof(updatedEmployee)));

        Username = updatedEmployee.Username;

        if (updatedEmployee.Supervisor?.Id == Supervisor?.Id)
            return;
        Supervisor?.TryRemoveSubordinate(this);
        updatedEmployee.Supervisor?.AddSubordinate(this);
    }

    public Problem AddProblem(string description)
    {
        if (Problems.Any(p => p.Description == description))
            throw new DomainException("Problem with given description is already assigned to an employee!");

        var problem = new Problem(description, this);
        _problems.Add(problem);
        return problem;
    }

    public void AddProblem(Problem problem)
    {
        if (problem is null) throw new DomainException("Adding problem can't be null!");

        if (_problems.Any(p => p.Equals(problem)))
            throw new DomainException($"Employee:{Id} is already assigned to problem:{problem.Id}");

        if (Problems.Any(p => p.Description == problem.Description))
            throw new DomainException("Problem with given description is already assigned to an employee!");

        _problems.Add(problem);
        problem.Employee = this;
    }

    public bool TryRemoveProblem(Problem problem)
    {
        return _problems.Remove(problem);
    }

    public void AddSubordinate(ISubordinate subordinate)
    {
        if (subordinate is null)
            throw new DomainException("Adding subordinate can't be null");

        if (_subordinates.Any(s => s.Id == subordinate.Id))
            throw new DomainException("Adding subordinate already assigned to ");

        subordinate.Supervisor?.TryRemoveSubordinate(subordinate);
        subordinate.Supervisor = this;
        _subordinates.Add(subordinate);
    }

    public void AddSubordinate(string username)
    {
        _subordinates.Add(new Employee(username, this));
    }

    public bool TryRemoveSubordinate(ISubordinate subordinate)
    {
        var success = _subordinates.Remove(subordinate);
        if (success)
        {
            Supervisor?.AddSubordinate(subordinate);
        }
        return success;
    }

    public void AddComment(Problem problem, string content)
    {
        problem.AddComment(content, this);
    }

    public virtual Report AddReport(string description)
    {
        if (Report is not null) throw new DomainException("Report is already created");
        Report = new Report(description, this);
        return Report;
    }
}