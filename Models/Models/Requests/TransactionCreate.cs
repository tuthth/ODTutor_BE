﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Requests
{
    public class TransactionCreate
    {
        [Required]
        [Range(10000, 100000000)]
        public decimal Amount { get; set; }
        [Required]
        public string RedirectUrl { get; set; } = null!;
        public Guid? SenderId { get; set; }
        public Guid? ReceiverId { get; set; }
    }
    public class BookingTransactionCreate : TransactionCreate
    {
        [Required]
        public Guid BookingId { get; set; }
    }
    public class CourseTransactionCreate : TransactionCreate
    {
        [Required]
        public Guid CourseId { get; set; }
    }
    public class WalletTransactionCreate : TransactionCreate
    {
        public int Choice { get; set; }
    }
}
