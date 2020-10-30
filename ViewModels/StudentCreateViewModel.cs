using Microsoft.AspNetCore.Http;
using StudentManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.ViewModels
{
    public class StudentCreateViewModel
    {
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

        [Display(Name = "图片")]
        public IFormFile Photo { get; set; }
    }
}
