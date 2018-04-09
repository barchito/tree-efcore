using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace tree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var context = new DatabaseContextFactory().CreateDbContext(null);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var parent = new Node{ Name = "Root"};

            var child2 = new Node{Name = "Child 2", Parent = parent};



            context.Nodes.Add(parent);
            context.Nodes.Add(new Node{Name = "Child 1", Parent = parent});
            context.Nodes.Add(child2);
            context.Nodes.Add(new Node{Name = "Child 3", Parent = parent});
            context.Nodes.Add(new Node{Name = "Child 4", Parent = parent});
            context.Nodes.Add(new Node{Name = "Child 5", Parent = parent});



            var children = Enumerable
                .Range(0,6)
                .Select(x => new Node{Name = $"Child 2 | Child {x}", Parent = child2})
                .ToList();

            children.ForEach(x => context.Nodes.Add(x));


            Enumerable
                .Range(0,10)
                .Select(x => new Node{Name = $"Child 2 | Child 3 | child {x}", Parent = children.ElementAt(3)})
                .ToList()
                .ForEach(x => context.Nodes.Add(x));
            


            var result = context.SaveChanges();

            // var children = context.Nodes.Include(x => x.Children).ToList();
            var node = context.Nodes.Include(x => x.Children).FirstOrDefault(x => x.Name =="Root");
            node.Children?.ToList().ForEach(x => {
                if(x.Children?.Count() > 0){
                    x.Children.ToList().ForEach(y => Console.WriteLine(y.Name));
                }else{
                    Console.WriteLine(x.Name);
                }
            });

            Console.WriteLine($"Result {result}");


            Console.Read();

        }
    }

    public class Database : DbContext
    {
        public Database(DbContextOptions options) : base(options)
        {
           
        }

         public DbSet<Node> Nodes { get; set; }
    }

    public class DatabaseContextFactory : IDesignTimeDbContextFactory<Database>
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {new ConsoleLoggerProvider((_, level) => level == LogLevel.Information, true)});        
        public Database CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Database>();
            //optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=tree;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=tree-test;User ID=sa;Password=<YourStrong!Passw0rd>");
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);

            return new Database(optionsBuilder.Options);
        }
    }

    public class Node
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public Node Parent { get; set; }
        public string Name { get; set; }

        public IEnumerable<Node> Children { get; set; }
    }
}
