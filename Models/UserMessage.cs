using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebEnterprise.Models
{
    public class UserMessage
    {
        [Key]
        public int Id { get; set; }

        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }

        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsSeen { get; set; }
    }
}