using System;
using System.ComponentModel.DataAnnotations;

namespace FZ.WriteLogs
{
    public class DataBaseSite
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Название сайта")]
        public string url { get; set; }

        [Display(Name = "Количество перезапуска")]
        public int RetryCount { get; set; }

        [Required]
        [Display(Name = "Кому отправлять логи")]
        public string message { get; set; }
    }
}