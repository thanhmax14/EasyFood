using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class IndexUserViewModels
    {
        public IndexUserViewModels()
        {
            userView = new UsersViewModel();
            Balance = new List<BalanceListViewModels>(); 
        }
        public UsersViewModel userView { get; set; }
        public List<BalanceListViewModels> Balance { get; set; }

    }
}
