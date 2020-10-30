using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.DataRepository
{
    public class MockStudentRepository : IStudentRepository
    {
        private List<Student> _students;

        public MockStudentRepository()
        {
            _students = new List<Student>
            {
                new Student(){ Id = 1, Name = "Ruby", ClassName = ClassNameEnum.FirstGrade, Email = "Rwby@Ruby.com"},
                new Student(){ Id = 2, Name = "Young", ClassName = ClassNameEnum.SecondGrade, Email = "Rwby@Young.com"},
                new Student(){ Id = 3, Name = "Black", ClassName = ClassNameEnum.GradeThree, Email = "Rwby@Black.com"}
            };
        }

        public IEnumerable<Student> GetAllStduents()
        {
            return _students;
        }

        public Student GetStudent(int id)
        {
           return  _students.FirstOrDefault(a => a.Id == id);
        }

        public Student Add(Student student)
        {
            student.Id = _students.Max(s => s.Id) + 1;
            _students.Add(student);
            return student;
        }

        public Student Update(Student updatestudent)
        {
            Student student = _students.FirstOrDefault(s => s.Id == updatestudent.Id);

            if(student != null)
            {
                student.Name = updatestudent.Name;
                student.Email = updatestudent.Email;
                student.ClassName = updatestudent.ClassName;
            }
            return student;
        }

        public Student Delete(int id)
        {
            Student student = _students.FirstOrDefault(s => s.Id == id);
            if(student != null)
            {
                _students.Remove(student);
            }

            return student;
        }
    }
}
