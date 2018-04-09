using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace tree
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var context = new DatabaseContextFactory().CreateDbContext(null);

            var parent = new Node{ Name = "Root"};

            var child = new Node{Name = "Child 1", Parent = parent};


            context.Nodes.Add(parent);
            context.Nodes.Add(child);

            var result = context.SaveChanges();

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
        public Database CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Database>();
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=tree;Trusted_Connection=True;MultipleActiveResultSets=true");

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
