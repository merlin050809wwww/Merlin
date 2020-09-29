﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FileStorage.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string IDUser { get; set; }
        public string Title { get; set; }
        [Display(Name = "Release Date")]
       // [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string Name { get; set; }
    }
}
