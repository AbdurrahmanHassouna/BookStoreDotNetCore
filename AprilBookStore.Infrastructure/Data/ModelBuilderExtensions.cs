using System;
using AprilBookStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AprilBookStore.Infrastructure.Data;
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            #region Author
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000054"),
                    Name = "Adam Kay",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000055"),
                    Name = "Daniel Kahneman",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000056"),
                    Name = "Paul Kalanithi",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000057"),
                    Name = "Russ Harris",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000059"),
                    Name = "Oliver Sacks",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000060"),
                    Name = "Michael Mosley",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author
                {
                    Id = new Guid("00000000-0000-0000-0000-000000000061"),
                    Name = "Jordan B. Peterson",
                    IsDeleted = false,
                    IsVisible = true,
                    CreatedDate = new DateTime(2023, 12, 8),
                    UpdatedDate = new DateTime(2023, 12, 8)
                },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000058"), Name = "Viktor E. Frankl", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000155"), Name = "Daniel J. Levitin", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000156"), Name = "Erik Larson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000062"), Name = "Atul Gawande", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000063"), Name = "Giulia Enders", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000064"), Name = "Norman Doidge", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000065"), Name = "Anthony William", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000066"), Name = "Keri Smith", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000067"), Name = "Dr. Natasha Campbell-McBride", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000068"), Name = "Austin Kleon", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000069"), Name = "Eline Snel", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000070"), Name = "Michael Greger", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000071"), Name = "Tina Payne Bryson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000072"), Name = "Juju Sundin", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000073"), Name = "American Psychiatric Association", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000074"), Name = "Philippa Perry", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000075"), Name = "American Psychological Association", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000076"), Name = "Dr. Eben Alexander", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000077"), Name = "Rebecca Skloot", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000078"), Name = "Stephen R. Covey", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000079"), Name = "C. G. Jung", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000080"), Name = "James Clear", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000081"), Name = "Dr. Sarah McKay", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000082"), Name = "Dr. Rhonda Patrick", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000083"), Name = "Peter Singer", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000084"), Name = "Helen Fisher", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000085"), Name = "Dr. Seuss", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000086"), Name = "Brene Brown", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000087"), Name = "Marie Kondō", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000088"), Name = "James Kerr", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000089"), Name = "Yuval Noah Harari", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000090"), Name = "Mark Manson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000091"), Name = "Eckhart Tolle", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000092"), Name = "Dr. Suess", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000093"), Name = "Robert Greene", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000094"), Name = "Simon Sinek", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000095"), Name = "Adam Grant", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000096"), Name = "Carol S. Dweck", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000097"), Name = "Malcolm Gladwell", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000098"), Name = "Ryan Holiday", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000099"), Name = "Tony Robbins", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000100"), Name = "J.K. Rowling", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000101"), Name = "Dr. Jordan B. Peterson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000102"), Name = "Dale Carnegie", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000103"), Name = "Susan Cain", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000104"), Name = "Cal Newport", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000105"), Name = "Gary Chapman", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000106"), Name = "Mark Manson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000107"), Name = "Charles Duhigg", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000108"), Name = "Napoleon Hill", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000109"), Name = "Daniel H. Pink", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000110"), Name = "Tara Westover", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000111"), Name = "Angela Duckworth", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000112"), Name = "Yuval Noah Harari", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000113"), Name = "Ray Dalio", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000114"), Name = "Jordan B. Peterson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000115"), Name = "Stephen R. Covey", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000116"), Name = "Paul Kalanithi", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000117"), Name = "J.D. Vance", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000118"), Name = "Atul Gawande", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000119"), Name = "Robert T. Kiyosaki", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000120"), Name = "Cheryl Strayed", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000121"), Name = "Tim Ferriss", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000122"), Name = "Dale Carnegie", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000123"), Name = "Robin Sharma", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000124"), Name = "Brene Brown", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000125"), Name = "Timothy Ferriss", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000126"), Name = "Jocko Willink", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000127"), Name = "David Goggins", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000128"), Name = "David Allen", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000129"), Name = "Yuval Noah Harari", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000130"), Name = "Gary John Bishop", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000131"), Name = "James Clear", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000132"), Name = "Nir Eyal", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000133"), Name = "Stephen R. Covey", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000134"), Name = "Charles Duhigg", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000135"), Name = "Daniel Kahneman", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000136"), Name = "James Clear", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000137"), Name = "Brené Brown", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000138"), Name = "Dale Carnegie", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000139"), Name = "Don Miguel Ruiz", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000140"), Name = "Mark Manson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000141"), Name = "Daniel H. Pink", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000142"), Name = "Gary Chapman", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000143"), Name = "Stephen R. Covey", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000144"), Name = "Brene Brown", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000145"), Name = "Dr. Shefali Tsabary", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000146"), Name = "Michael A. Singer", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000147"), Name = "Dale Carnegie", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000148"), Name = "Gary Chapman", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000149"), Name = "Simon Sinek", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Author { Id = new Guid("00000000-0000-0000-0000-000000000150"), Name = "Mark Manson", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") });
            #endregion

            #region Category
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = new Guid("00000000-0000-0000-0000-000000000001"), Name = "Medical", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Category { Id = new Guid("00000000-0000-0000-0000-000000000002"), Name = "Science-Geography", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") },
                new Category { Id = new Guid("00000000-0000-0000-0000-000000000003"), Name = "Art-Photography", IsDeleted = false, IsVisible = true, CreatedDate = DateTime.Parse("2023-12-08"), UpdatedDate = DateTime.Parse("2023-12-08") }
            );
            #endregion

            #region Book
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000314"), Name = "This is Going to Hurt", Format = "Paperback", ISBN = "9781509858637", Price = 7.60M, ImgPath = "/book-covers/Medical/0000001.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000054"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 9, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000315"), Name = "Thinking, Fast and Slow", Format = "Paperback", ISBN = "9780141033570", Price = 11.50M, ImgPath = "/book-covers/Medical/0000002.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000055"), BookStar = 1.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 1, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000316"), Name = "When Breath Becomes Air", Format = "Paperback", ISBN = "9781784701994", Price = 9.05M, ImgPath = "/book-covers/Medical/0000003.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000056"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 8, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000317"), Name = "The Happiness Trap", Format = "Paperback", ISBN = "9781845298258", Price = 8.34M, ImgPath = "/book-covers/Medical/0000004.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000057"), BookStar = 3.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 6, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000318"), Name = "Man's Search For Meaning", Format = "Paperback", ISBN = "9781846041242", Price = 9.66M, ImgPath = "/book-covers/Medical/0000005.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000058"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 1, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000319"), Name = "The Man Who Mistook His Wife for a Hat", Format = "Paperback", ISBN = "9780330523622", Price = 5.92M, ImgPath = "~/book-covers/Medical/0000006.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000059"), BookStar = 5.0M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 6, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000320"), Name = "The 8-week Blood Sugar Diet", Format = "Paperback", ISBN = "9781780722405", Price = 8.85M, ImgPath = "/book-covers/Medical/0000007.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000060"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 4, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000321"), Name = "The Subtle Art of Not Giving a F*ck", Format = "Paperback", ISBN = "9780062457714", Price = 8.06M, ImgPath = "/book-covers/Medical/0000008.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000061"), BookStar = 4.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 6, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000322"), Name = "Educated", Format = "Paperback", ISBN = "9780099511021", Price = 8.73M, ImgPath = "/book-covers/Medical/0000009.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000062"), BookStar = 3.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 8, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000323"), Name = "The Body Keeps the Score", Format = "Paperback", ISBN = "9780141978611", Price = 10.27M, ImgPath = "/book-covers/Medical/0000010.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000063"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 2, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000324"), Name = "Why We Sleep", Format = "Paperback", ISBN = "9780141983769", Price = 9.20M, ImgPath = "/book-covers/Medical/0000011.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000064"), BookStar = 3.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 3, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000325"), Name = "The Immortal Life of Henrietta Lacks", Format = "Paperback", ISBN = "9780330533447", Price = 6.82M, ImgPath = "~/book-covers/Medical/0000012.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000065"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 5, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000326"), Name = "Sapiens: A Brief History of Humankind", Format = "Paperback", ISBN = "9780099590087", Price = 10.15M, ImgPath = "~/book-covers/Medical/0000013.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000066"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 7, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000327"), Name = "Bad Pharma", Format = "Paperback", ISBN = "9780007498086", Price = 7.84M, ImgPath = "/book-covers/Medical/0000014.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000067"), BookStar = 3.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 2, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000328"), Name = "The Checklist Manifesto", Format = "Paperback", ISBN = "9781846683145", Price = 8.34M, ImgPath = "/book-covers/Medical/0000015.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000068"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 3, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000329"), Name = "The Emperor of All Maladies", Format = "Paperback", ISBN = "9780007250929", Price = 8.49M, ImgPath = "/book-covers/Medical/0000016.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000069"), BookStar = 4.5M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 5, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) },
                new Book { Id = new Guid("00000000-0000-0000-0000-000000000330"), Name = "The Gene", Format = "Paperback", ISBN = "9780099584574", Price = 12.20M, ImgPath = "/book-covers/Medical/0000017.jpg", CategoryId = new Guid("00000000-0000-0000-0000-000000000001"), AuthorId = new Guid("00000000-0000-0000-0000-000000000070"), BookStar = 2.1M, PublicationDate = new DateTime(2023, 12, 8), Description = "No Description AVAILABLE", QuantityInStock = 2, IsDeleted = false, IsVisible = true, CreatedDate = new DateTime(2023, 12, 8), UpdatedDate = new DateTime(2023, 12, 8) }
            );
            #endregion

            #region ApplicationUser
            string roleId = "02174cf0–9412–4cfe-afbf-59f706d72cf6";
            string userId = "341743f0-asd2–42de-afbf-59kmkkmk72cf6";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = roleId,
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = roleId
                }
            );

            ApplicationUser user = new ApplicationUser
            {
                UserName = "Abdo",
                NormalizedEmail = "TEST@TEST.COM",
                Email = "test@test.com",
                NormalizedUserName = "ABDO",
                Id = userId,
                EmailConfirmed = true
            };

            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, "123@Bdu456");

            modelBuilder.Entity<ApplicationUser>().HasData(user);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId,
            });
            #endregion
        }
    }

