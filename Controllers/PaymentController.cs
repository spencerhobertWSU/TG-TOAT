using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using TGTOAT.Data;
using TGTOAT.Helpers;
using TGTOAT.Models;

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IAuthentication _auth;

    public PaymentController(IConfiguration configuration, IAuthentication auth)
    {
        _configuration = configuration;
        _auth = auth;
    }

    public ActionResult Index()
    {
        var model = new PaymentViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            AmountDue = 500.00m,
            Courses = new List<Courses>
            {
                new Courses { CourseName = "Math", NumberOfCredits = 3 },
                new Courses { CourseName = "Science", NumberOfCredits = 4 }
            },
            PublishableKey = _configuration["StripePublishableKey"]
        };

        return View(model);
    }

    public ActionResult Checkout(decimal amountDue)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(amountDue * 100), // For some reason Stripe requires the amount in cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Course Payment",
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
            CancelUrl = Url.Action("Index", "Payment", null, Request.Scheme),
        };

        var service = new SessionService();
        Session session = service.Create(options);

        return Redirect(session.Url);
    }

    public ActionResult Success()
    {
        return RedirectToAction("Payment", "User", new { didSucceed = true });
    }

    public ActionResult Failure()
    {
        return RedirectToAction("Payment", "User", new { didSucceed = false });
    }
}
