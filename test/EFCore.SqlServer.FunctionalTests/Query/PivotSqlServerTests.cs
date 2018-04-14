using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class PivotSqlServerTests : IClassFixture<PivotSqlServerTests.SqlServerPivotFixture>
    {
        public PivotSqlServerTests(SqlServerPivotFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        private SqlServerPivotFixture Fixture { get; }

        protected PivotSqlContext CreateContext() => (PivotSqlContext)Fixture.CreateContext();

        public class SqlServerPivotFixture : SharedStoreFixtureBase<DbContext>
        {
            protected override string StoreName { get; } = "PivotSqlServerTests";
            protected override Type ContextType { get; } = typeof(PivotSqlContext);
            protected override ITestStoreFactory TestStoreFactory => SqlServerTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                base.AddOptions(builder);
                return builder.ConfigureWarnings(w => w.Ignore(RelationalEventId.QueryClientEvaluationWarning));
            }

            protected override void Seed(DbContext context)
            {
                context.Database.EnsureCreated();

                var school1 = new School { Name = "Wisconsin", Conference = "BigTen", Attendence = 71534 };
                var school2 = new School { Name = "Minnesota", Conference = "BigTen", Attendence = 54782 };
                var school3 = new School { Name = "Oklahoma", Conference = "BigTwelve", Attendence = 82671 };
                var school4 = new School { Name = "Alabama", Conference = "SEC", Attendence = 63256 };

                var player1 = new Player { Name = "John", Position="QB", Weight=210, Year="SR", Birthday = DateTime.Parse("1/1/2000"), School = school1};
                var player2 = new Player { Name = "James", Position = "WR", Weight = 185, Year = "FR", Birthday = DateTime.Parse("2/1/2000"), School = school1 };
                var player3 = new Player { Name = "Robert", Position = "OL", Weight = 310, Year = "JR", Birthday = DateTime.Parse("3/1/2000"), School = school2 };
                var player4 = new Player { Name = "Mike", Position = "LB", Weight = 250, Year = "SO", Birthday = DateTime.Parse("4/1/2000"), School = school2 };
                var player5 = new Player { Name = "Steve", Position = "QB", Weight = 205, Year = "SR", Birthday = DateTime.Parse("5/1/2000"), School = school3 };
                var player6 = new Player { Name = "Mary", Position = "WR", Weight = 165, Year = "SO", Birthday = DateTime.Parse("6/1/2000"), School = school3 };
                var player7 = new Player { Name = "Nancy", Position = "RB", Weight = 150, Year = "FR", Birthday = DateTime.Parse("7/1/2000"), School = school4 };
                var player8 = new Player { Name = "Brenda", Position = "LB", Weight = 175, Year = "FR", Birthday = DateTime.Parse("8/1/2000"), School = school4 };
                var player9 = new Player { Name = "Paul", Position = "QB", Weight = 190, Year = "Jr", Birthday = DateTime.Parse("1/1/2000"), School = school1 };
                var player10 = new Player { Name = "Chris", Position = "WR", Weight = 250, Year = "SO", Birthday = DateTime.Parse("2/1/2000"), School = school1 };
                var player11 = new Player { Name = "Peter", Position = "OL", Weight = 330, Year = "SR", Birthday = DateTime.Parse("3/1/2000"), School = school2 };
                var player12 = new Player { Name = "Neil", Position = "LB", Weight = 275, Year = "FR", Birthday = DateTime.Parse("4/1/2000"), School = school2 };
                var player13 = new Player { Name = "Nate", Position = "QB", Weight = 225, Year = "SR", Birthday = DateTime.Parse("5/1/2000"), School = school3 };
                var player14 = new Player { Name = "Julie", Position = "WR", Weight = 150, Year = "Jr", Birthday = DateTime.Parse("6/1/2000"), School = school3 };
                var player15 = new Player { Name = "Bob", Position = "RB", Weight = 180, Year = "So", Birthday = DateTime.Parse("7/1/2000"), School = school4 };
                var player16 = new Player { Name = "Eric", Position = "LB", Weight = 205, Year = "SR", Birthday = DateTime.Parse("8/1/2000"), School = school4 };


                ((PivotSqlContext)context).Schools.AddRange(school1, school2, school3, school4);
                ((PivotSqlContext)context).Players.AddRange(player1, player2, player3, player4, player5, player6, player7, player8, player9, player10, player11, player12, player13, player14, player15, player16);

                context.Database.ExecuteSqlCommand(
    @"CREATE VIEW View_SchoolPlayer AS 
        SELECT s.Name as SchoolName, p.name as PlayerName
        from schools s
        JOIN players p on s.id = p.schoolid");

                context.SaveChanges();
            }
        }

        public class School
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Conference { get; set; }
            public double Attendence { get; set; }
        }

        public class Player
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public int Weight { get; set; }
            public string Year { get; set; }
            public DateTime Birthday { get; set; }

            public int SchoolId { get; set; }
            public School School { get;set; }
        }

        public class PivotPlayerWeight
        {
            public double? QB { get; set; }
            public double? RB { get; set; }
            public double? WR { get; set; }
            public double? LB { get; set; }
            public double? OL { get; set; }
        }

        public class PivotSchool
        {
            public double? Wisconsin { get; set; }
            public double? Minnesota { get; set; }
            public double? Oklahoma { get; set; }
            public double? Alabama { get; set; }
        }

        public class SchoolPositionWeight
        {
            public int? Quarterback { get; set; }
            public int? WideReceiver { get; set; }
            public int? LineBacker { get; set; }
            public int? RunningBack { get; set; }
            public int? OffensiveLine { get; set; }

            public string SchoolName { get; set; }
        }

        public class SchoolPlayer
        {
            public string SchoolName { get; set; }
            public string PlayerName { get; set; }
        }

       #region Tests

        [Fact]
        public void PivotDev()
        {
            using (var context = CreateContext())
            {
                var t = context.Schools.Select(
                    s => new
                    {
                        s.Name,
                        s.Id
                    }).ToList();

                //var a = context.SchoolPlayers.ToList();

                //var a = context.Schools.Select(s => new {  s.Id, s.Name }).ToList() ;
                //   var a = context.Schools.ToList();

                //var r = context.Schools.Pivot(b => b.Conference, bl => bl.Sum(b => b.Attendence), b => new PivotSchool()).Select(p => p.Wisconsin).ToList();


                //var r3 = context.Schools.GroupBy(b => b.Conference).ToList();
                // var r3 = context.Schools.GroupBy(b => b.Conference).Select(g => g.Key).ToList();

                //    var r4 = context.Schools.Select(s => new PivotSchool() {  Alabama = s.Attendence }).ToList() ;

                // context.Schools.ToList();
                //    var a = new DateTime(2000, 1, 1);
                //var results = context.Schools.Where(s => s.Attendence > 1).GroupBy(foo => foo.Name).ToList();
                //    var results = context.Players.GroupBy(p => EF.Functions.DateDiffDay(a, p.Birthday))
                //       .Select(d => d.Sum(d1 => d1.Weight)).ToList();
                //  .Select(d => new { Dw = (DayOfWeek)d.Key, Value = d.Sum(d1 => d1.Weight) }).ToList();

                //     var results2 = context.Schools.Pivot<School, PivotSchool>(s => s.Name,
                //                                 sl => sl.Sum(s => s.Attendence)).ToList();

                /* var results = context.Schools.Join(
                     context.Players, s => s.Id, p => p.SchoolId, (s, p) => new
                     {
                         SchoolName = s.Name,
                         PlayerName = p.Name,
                         p.Position,
                         p.Weight
                     }).GroupBy(foo => foo.Position).ToList();
                  .Select(
                      b => new
                      {
                          b.Key,
                      }).ToList();*/


            }
        }

        [Fact]
        public void PivotSimpleSelectEntity()
        {
            using (var context = CreateContext())
            {
                //todo - the shcool qsre doesn't need to be captured as requiring materialization in RequiresMaterializationExpressionVisitor
                var results = context.Schools.Pivot<School, PivotSchool>(s => s.Name,
                                                     sl => sl.Sum(s => s.Attendence)).ToList();

                Assert.Equal(4, results.Count);
                Assert.Equal(63256, results[0].Alabama);
                Assert.Equal(54782, results[0].Minnesota);
                Assert.Equal(82671, results[0].Oklahoma);
                Assert.Equal(71534, results[0].Wisconsin);
            }
        }

        [Fact]
        public void PivotSimpleSelectAnonymous()
        {
            using (var context = CreateContext())
            {
                /*var results = context.Players
                    .Pivot<Player, PivotPlayerWeight>(p => p.Position, pl => pl.Average(p => p.Weight))
                    .Select(pp => pp.WR)
                    .ToList();

                Assert.Equal(1, results.Count);
                Assert.Equal(175, results[0]);*/
                
                //Assert.Equal()

                var results = context.Schools.Pivot<School, PivotSchool>(s => s.Name, sl => sl.Max(s => s.Attendence))
                                                .Select(p => p.Wisconsin).ToList();

                Assert.Equal(1, results.Count);
                Assert.Equal(71534, results[0]);
            }
        }

        [Fact]
        public void PivotJoinSelectAnonymousWithAltColumnNames()
        {
            using (var context = CreateContext())
            {
                /* var results = context.Schools.Join(
                         context.Players, s => s.Id, p => p.SchoolId, (s, p) => new
                         {
                             SchoolName = s.Name,
                             p.Position,
                             p.Weight
                         }).Pivot(b => b.Position,
                         bl => bl.Max(b => b.Weight),
                         b => new SchoolPositionWeight())
                     .Select(p => new { p.Quarterback, p.LineBacker, p.OffensiveLine, p.RunningBack, p.WideReceiver, p.SchoolName }).ToList();*/

                var results = context.Players.Select(p =>
                        new {
                            SchoolName = p.School.Name,
                            p.Position,
                            p.Weight
                        }).Pivot(b => b.Position,
                        bl => bl.Max(b => b.Weight),
                        b => new SchoolPositionWeight())
                    .Select(p => new { p.Quarterback, p.LineBacker, p.OffensiveLine, p.RunningBack, p.WideReceiver, p.SchoolName }).ToList();


                Assert.Equal(4, results.Count);
            }
        }

        [Fact]
        public void JoinAfterPivotSelectAnonymous()
        {
            using (var context = CreateContext())
            {
                var results = context.Schools.Pivot<School, PivotSchool>(
                                s => s.Name,
                                sl => sl.Sum(s => s.Attendence))
                                .Join(context.Schools, ps => ps.Wisconsin, s => s.Attendence, (ps, s) => new { ps, s })
                                .Select(a => new { a.ps.Wisconsin, a.s.Name })
                                .ToList();

                Assert.Equal(1, results.Count);
                Assert.Equal(results[0].Name, "Wisconsin");
                Assert.Equal(results[0].Wisconsin, 71534);
            }
        }

        //todo - most stuff after pivot operator

        [Fact]
        public void PivotJoinExtraColumnsSelectAnonymous()
        {
            //test where the pivot source has extra columns that are not the pivot column or aggregate
            //https://www.red-gate.com/simple-talk/sql/t-sql-programming/questions-about-pivoting-data-in-sql-server-you-were-too-shy-to-ask/
            //Can I group data by more than one column when I use the PIVOT operator?
        }

        /*  [Fact]
          public void PivotJoinCountSelectAnonymous()
          {
              var results = context.Schools.Join(
                  context.Players, s => s.Id, p => p.SchoolId, (s, p) => new
                  {
                      SchoolName = s.Name,
                      PlayerName = p.Name,
                      p.Position,
                      p.Year
                  }).Pivot(b => b.Position,
                  bl => bl.Count((b => b.Year),
                          b => new SchoolPositionWeight())
                      .Select(p => p.QB).ToList();

              Assert.Equal(4, results.Count);
          }*/

        /*
         *    [Fact]
        public void PivotDev()
        {
            using (var context = CreateContext())
            {
                // var q = context.Books.All(b => b.Id == 1);

                //   var q = context.Books.Sum(b => b.Sales);

                //   var r2 = context.Books.Average(b => b.Sales);

                //                var r3 = context.Books.GroupBy(b => b.Author).Select(g => g.Key).ToList();

                //var r = context.Books.Pivot(b => b.Author, bl => bl.Sum(b => b.Sales), b => new PivotBook()).ToList();

               //    var r = context.Books.Pivot(b => b.Author, bl => bl.Sum(b => b.Sales), b => new PivotBook()).Select(p => p.King).ToList();

                /*    var r3 = context.Customers.Join(
                        context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new
                        {
                            c.LastName,
                            o.ProductId,
                            o.Name,
                            o.QuantitySold
                        }).Join(
                        context.Products, o => o.ProductId, p => p.Id, (co, p) => new
                        {
                            co.LastName,
                            co.QuantitySold,
                            p.Name
                        }).ToList();*/


        /*  var r4 = context.Customers.Join(
              context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new
              {
                  Arf = c.LastName,
                  o.QuantitySold
              }).GroupBy(co => co.Arf).Select(g => new { qs = g.Max(co => co.QuantitySold), ln = g.Key }).ToList();*/


        /*   var r2 = context.Customers.Join(
                   context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new
                   {
                       c.LastName,
                       o.Name,
                       o.QuantitySold
                   })
               .Pivot(co => co.LastName, cos => cos.Sum(co => co.QuantitySold), co => new CustomerOrderPivot()).ToList();//.Select(cop => new { cop.One, cop.Two, cop.Three }).ToList();
               */
        /*  var r2 = context.Customers.Join(
                  context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new
                  {
                      c.LastName,
                      o.Name,
                      o.QuantitySold
                  })
              .Pivot(co => co.LastName, cos => cos.Sum(co => co.QuantitySold), co => new CustomerOrderPivot()).Select(cop => new { cop.One, cop.Two, cop.Three }).ToList();
            */

        //   var q = (from b in context.Books
        //          group b.Sales by b.Author into g
        //         select new { Author = g.Key, TotalSales = g.Sum() }).ToList();

        //    context.Books.GroupBy(b => b.Author)
        //      .SelectMany(b => b.)
        //      var a = context.Books.Union(context.Books).Select(b => b.Author).ToList();

        // var u = context.Books.Select(b => b.Id).Union(context.Products.Select(p => p.Id)).ToList();

        //var r2 = context.Books.Pivot(b => b.Author, bl => bl.Sum(b => b.Sales), b => new { BookType = "", King = "", Rowling = "" });
        //var r3 = context.Books.Pivot<Book, PivotBook, string, double>(b => b.Author, bl => bl.Sum(b => b.Sales));
        /*   }
       }
       */
        #endregion

        protected class PivotSqlContext : DbContext
        {
            #region DbSets

            public DbSet<School> Schools { get; set; }
            public DbSet<Player> Players { get; set; }
            public DbQuery<SchoolPlayer> SchoolPlayers { get; set; }
            
            #endregion

            public PivotSqlContext(DbContextOptions options)
                : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Query(typeof(PivotSchool));

                modelBuilder.Query(typeof(PivotPlayerWeight));

                var psp = modelBuilder.Query(typeof(SchoolPositionWeight));
                psp.Property("Quarterback").HasColumnName("qb").IsPivot();
                psp.Property("RunningBack").HasColumnName("rb").IsPivot();
                psp.Property("WideReceiver").HasColumnName("wr").IsPivot();
                psp.Property("LineBacker").HasColumnName("lb").IsPivot();
                psp.Property("OffensiveLine").HasColumnName("ol").IsPivot();

                modelBuilder.Query<SchoolPlayer>().ToView("View_SchoolPlayer");

                //.Property(v => v.SchoolName).HasColumnName("SchoolName");
            }
        }
    }
}
