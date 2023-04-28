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
        public string Url { get; set; }

        [Display(Name = "Количество перезапуска")]
        public int RetryCount { get; set; }

        [Required]
        [Display(Name = "Кому отправлять логи")]
        public string Message { get; set; }

		[Display(Name = "Активность отслеживания")]
		public bool Active { get; set; }
    }
}