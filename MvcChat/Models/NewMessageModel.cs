using System.ComponentModel.DataAnnotations;

namespace MvcChat.Models
{
    public class NewMessageModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User name required")]
        [StringLength(30, ErrorMessage = "User name should not be longer than 30 symbols")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Message required")]
        [StringLength(200, ErrorMessage = "Message should not be longer than 200 symbols")]
        [Display(Name = "Message")]
        public string Message { get; set; }
        
        /// <summary>
        /// last message id being displayed for current user
        /// </summary>
        public int? LastCheckedId { get; set; }        
    }
}