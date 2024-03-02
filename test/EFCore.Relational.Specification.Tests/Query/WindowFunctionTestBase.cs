// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query;

public abstract class WindowFunctionTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : SharedStoreFixtureBase<DbContext>, new()
{
    protected TFixture Fixture { get; }

    protected WindowFunctionTestBase(TFixture fixture)
    {
        Fixture = fixture;
    }

    protected WindowFunctionContext CreateContext()
     => (WindowFunctionContext)Fixture.CreateContext();

    #region Model

    public class Employee
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public decimal Salary { get; set; }
        public int WorkExperience { get; set; }
    }

    public class NullTestEmployee
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public decimal? Salary { get; set; }
        public int WorkExperience { get; set; }
    }
    public class WindowFunctionContext : PoolableDbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<NullTestEmployee> NullTestEmployees { get; set; }

        public WindowFunctionContext(DbContextOptions options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<NullTestEmployee>().Property(e => e.Salary).HasColumnType("decimal(10,2)");
        }
    }

    public abstract class WindowFunctionFixture : SharedStoreFixtureBase<DbContext>
    {
        protected override Type ContextType { get; } = typeof(WindowFunctionContext);

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override bool ShouldLogCategory(string logCategory)
            => logCategory == DbLoggerCategory.Query.Name;

        protected override void Seed(DbContext context)
        {
            var ctx = context as WindowFunctionContext;

            context.Database.EnsureCreatedResiliently();

            var emp1 = new Employee
            {
                EmployeeId = 1,
                Name = "Luke Sykwalker",
                Salary = 100000.0m,
                WorkExperience = 10,
                DepartmentName = "IT"
            };

            var emp2 = new Employee
            {
                EmployeeId = 2,
                Name = "Darth Vader",
                Salary = 500000.0m,
                WorkExperience = 15,
                DepartmentName = "Security"
            };

            var emp3 = new Employee
            {
                EmployeeId = 3,
                Name = "Emperor Palpatine",
                Salary = 1000000.0m,
                WorkExperience = 20,
                DepartmentName = "Corporate"
            };

            var emp4 = new Employee
            {
                EmployeeId = 4,
                Name = "Leia Organa",
                Salary = 200000.0m,
                WorkExperience = 12,
                DepartmentName = "IT"
            };

            var emp5 = new Employee
            {
                EmployeeId = 5,
                Name = "Boba Fett",
                Salary = 50000.0m,
                WorkExperience = 4,
                DepartmentName = "Security"
            };

            var emp6 = new Employee
            {
                EmployeeId = 6,
                Name = "Han Solo",
                Salary = 350000.0m,
                WorkExperience = 8,
                DepartmentName = "Sales"
            };

            var emp7 = new Employee
            {
                EmployeeId = 7,
                Name = "Jabba the Hutt",
                Salary = 1750000.0m,
                WorkExperience = 18,
                DepartmentName = "Sales"
            };

            ctx.Employees.AddRange(emp1, emp2, emp3, emp4, emp5, emp6, emp7);

            var nullEmp1 = new NullTestEmployee
            {
                EmployeeId = 1,
                Name = "Battle Droid",
                Salary = null,
                WorkExperience = 1,
                DepartmentName = "Security"
            };

            ctx.NullTestEmployees.AddRange(nullEmp1);

            context.SaveChanges();
        }
    }

    #endregion

    #region Tests

    #region Aggregates

    [ConditionalFact]
    public virtual void Max_Basic()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            MaxSalary = EF.Functions.Over().Max(e.Salary)
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(1750000.0m, results[0].MaxSalary);
    }

    [ConditionalFact]
    public virtual void Max_Null()
    {
        using var context = CreateContext();

        var results = context.NullTestEmployees.Select(e => new
        {
            e.Id,
            e.Name,
            MaxSalary = EF.Functions.Over().Max(e.Salary)
        }).ToList();

        Assert.Equal(1, results.Count);
        Assert.Null(results[0].MaxSalary);
    }

    [ConditionalFact]
    public virtual void Min_Basic()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            MinSalary = EF.Functions.Over().Min(e.Salary)
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(50000.00m, results[0].MinSalary);
    }

    [ConditionalFact]
    public virtual void Min_Null()
    {
        using var context = CreateContext();

        var results = context.NullTestEmployees.Select(e => new
        {
            e.Id,
            e.Name,
            MinSalary = EF.Functions.Over().Min(e.Salary)
        }).ToList();

        Assert.Equal(1, results.Count);
        Assert.Null(results[0].MinSalary);
    }

    [ConditionalFact]
    public virtual void Count_Star_Basic()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            Count = EF.Functions.Over().Count()
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(7, results[0].Count);
    }

    [ConditionalFact]
    public virtual void Count_Col_Basic()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            Count = EF.Functions.Over().Count(e.Id)
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(7, results[0].Count);
    }

    #endregion

    #region WindowOverExpression Equality tests

    [ConditionalFact]
    public virtual void Multiple_Aggregates_Basic_NoDup_Query()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            MaxSalary = EF.Functions.Over().Max(e.Salary),
            MinSalary = EF.Functions.Over().Min(e.Salary)
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(1750000.0m, results[0].MaxSalary);
        Assert.Equal(50000.00m, results[0].MinSalary);
    }

    [ConditionalFact]
    public virtual void Multiple_Aggregates_Basic_Dup_Query()
    {
        using var context = CreateContext();

        var results = context.Employees.Select(e => new
        {
            e.Id,
            e.Name,
            MaxSalary1 = EF.Functions.Over().Max(e.Salary),
            MaxSalary2 = EF.Functions.Over().Max(e.Salary)
        }).ToList();

        Assert.Equal(7, results.Count);
        Assert.Equal(1750000.0m, results[0].MaxSalary1);
        Assert.Equal(1750000.0m, results[0].MaxSalary2);
    }

    #endregion


    #endregion
}

