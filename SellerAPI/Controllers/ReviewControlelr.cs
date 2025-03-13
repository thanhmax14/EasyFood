//using BusinessLogic.Services.Reviews;
//using Microsoft.AspNetCore.Mvc;
//using Repository.ViewModels;

//namespace SellerAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReviewControlelr : ControllerBase
//    {
//        private readonly IReviewService _reviewService;

//        public ReviewControlelr(IReviewService reviewService)
//        {
//            _reviewService = reviewService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var list = new List<ReivewViewModel>();

//            var review = await _reviewService.ListAsync();

//            if (review.Any())
//            {
//                foreach (var item in review)
//                {
//                    list.Add(new ReivewViewModel
//                    {
//                        ID = item.ID,
//                        Cmt = item.Cmt,
//                        Datecmt = item.Datecmt,
//                        Relay = item.Relay,
//                        DateRelay = item.DateRelay,
//                        Status = item.Status,
//                        Rating = item.Rating,
//                        UserID = item.UserID,
//                        ProductID = item.ProductID,
//                        Product = item.Product,

//                    });
//                }
//                return Ok(list);
//            }
//            return BadRequest(false);
//        }
//    }
//}
