namespace Repository.ViewModels
{
    public class WithdrawAdminListViewModel
    {
        public int No { get; set; }
        public Guid ID { get; set; }
        public decimal MoneyChange { get; set; } // amount
        public DateTime? StartTime { get; set; } // transaction date
        public DateTime? DueTime { get; set; }
        public string? Description { get; set; } // bank and bank account
        public string UserID { get; set; }
        public string Status { get; set; } // Pending // Accept // Reject
        public string Method { get; set; } // Withdraw
        public bool DisPlay { get; set; }
        public string UserName { get; set; } // username
    }
}
