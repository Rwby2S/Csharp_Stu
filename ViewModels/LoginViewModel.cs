using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManager.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 选中则使用持久性Cooke,而未选中则为会话Cookie
        /// </summary>
        [Display(Name ="记住我")]
        public bool RemenberMe { get; set; }
    }
}
