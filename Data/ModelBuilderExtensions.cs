using Microsoft.EntityFrameworkCore;
using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student(){ Id = 1, Name = "Ruby", ClassName = ClassNameEnum.FirstGrade, Email = "Rwby@Ruby.com"},
                new Student(){ Id = 2, Name = "Young", ClassName = ClassNameEnum.SecondGrade, Email = "Rwby@Young.com"},
                new Student(){ Id = 3, Name = "Black", ClassName = ClassNameEnum.GradeThree, Email = "Rwby@Black.com"}
                );
        }
    }
}
