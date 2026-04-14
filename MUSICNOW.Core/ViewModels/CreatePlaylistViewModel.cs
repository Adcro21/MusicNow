using System.ComponentModel.DataAnnotations;

namespace MUSICNOW.Core.ViewModels
{
    public class CreatePlaylistViewModel
    {
        [Display(Name = "Tên Playlist")]
        [Required(ErrorMessage = "Vui lòng nhập tên cho playlist")]
        public string Name { get; set; }
    }
}