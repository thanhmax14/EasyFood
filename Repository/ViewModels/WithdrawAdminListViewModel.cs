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
        public string Status { get; set; } // PROCESSING // Success // CANCELLED
        public string Method { get; set; } // Withdraw
        public string UserName { get; set; } // username

        // 🛠 Tự động tách dữ liệu từ Description
        //Phạm Quang Thành&939371017&VietinBank&5000000
        public string? Recipient => GetPartFromDescription(0);
        public string? BankAccount => GetPartFromDescription(1);
        public string? BankName => GetPartFromDescription(2);
        public string? Amount => GetPartFromDescription(3);

        private string? GetPartFromDescription(int index)
        {
            if (string.IsNullOrEmpty(Description)) return null;
            var parts = Description.Split('&'); // Cắt dữ liệu theo ký tự "&"
            return parts.Length > index ? parts[index].Trim() : null;
        }
    }
}
