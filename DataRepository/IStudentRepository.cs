using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.DataRepository
{
    public interface IStudentRepository
    {
        Student GetStudent(int id);

        //IEnumerable类型是指数据在内存中
        /// <summary>
        /// 获取所有的学生信息
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        IEnumerable<Student> GetAllStduents();
       /// <summary>
       /// 添加一名学生信息
       /// </summary>
       /// <param name="student"></param>
       /// <returns></returns>
        public Student Add(Student student);
        /// <summary>
        /// 更新一名学生的信息
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public Student Update(Student updatestudent);
        /// <summary>
        /// 删除一名学生的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Student Delete(int id);
    }
}
