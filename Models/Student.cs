using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.Models
{
    /**
     *    在Name属性上添加了Required属性,它会判断Name中的值,如果该Name中的值为空
     * 或者属性不存在,则会验证失败,使用ModelState.IsValid属性会检查验证是失败还是成功
     */
    public class Student
    {
        public int Id { get; set; }    
        [Display(Name = "姓名")]
        //自定义验证信息
        [Required(ErrorMessage = "请输入姓名"), MaxLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
        public string Name { get; set; }

        [Display(Name = "班级信息")]
        [Required]
        public ClassNameEnum? ClassName { get; set; }

        [Required]
        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        public string PhotoPath { get; set; }

       // public int deleteProperty { get; set; }
    }
}
