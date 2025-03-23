namespace Repository.ViewModels
{
    public class IndexUserViewModels
    {
        public IndexUserViewModels()
        {
            userView = new UsersViewModel();
            Balance = new List<BalanceListViewModels>();
            Reivew = new List<ReivewViewModel>();
        }
        public UsersViewModel userView { get; set; }
        public List<BalanceListViewModels> Balance { get; set; }
        public List<ReivewViewModel> Reivew { get; set; }
        public decimal BalanceUser { get; set; } = 0;
        public List<OrderViewModel> OrderViewodels { get; set; }

    }
}
