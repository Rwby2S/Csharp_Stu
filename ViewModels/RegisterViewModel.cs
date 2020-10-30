using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="邮箱地址")]
        public string Email { get; set; }

        /// <summary>
        /// DataType特性： 
        ///         主要作用是指比数据库内部类型更具体的数据类型
        ///     比如日期、时间、电话号码、货币和邮箱地址等。
        ///     但是DataType不提供任何验证，它主要服务于我们的视图文件
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", 
            ErrorMessage = "密码与确认密码不一致，请重新输入")]
        public string ConfirmPassword { get; set; }
    }
}
