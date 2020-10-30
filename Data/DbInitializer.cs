using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Students.Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
           {
                new Student(){ Id = 1, Name = "Ruby", ClassName = ClassNameEnum.FirstGrade, Email = "Rwby@Ruby.com"},
                new Student(){ Id = 2, Name = "Young", ClassName = ClassNameEnum.SecondGrade, Email = "Rwby@Young.com"},
                new Student(){ Id = 3, Name = "Black", ClassName = ClassNameEnum.GradeThree, Email = "Rwby@Black.com"}
           };
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();
        }
    }
}
