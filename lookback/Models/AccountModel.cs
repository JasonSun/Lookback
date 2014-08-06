using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lookback.Models
{
    [Table("Account")]
    public class AccountModel
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        [DisplayName("微博用户id")]
        public string WeiboId { get; set; }

        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("微博昵称")]
        public string NickName { get; set; }

        [DisplayName("用户邮箱")]
        public string Email { get; set; }

        [DisplayName("微博头像50*50")]
        public string Avatar50Url { get; set; }

        [DisplayName("微博头像180*180")]
        public string Avatar180Url { get; set; }

        [Required]
        [DisplayName("微博用户创建日期")]
        public string CreateDate { get; set; }
    }
}