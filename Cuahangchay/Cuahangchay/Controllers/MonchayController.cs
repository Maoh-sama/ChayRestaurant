using Microsoft.AspNetCore.Mvc;
using Cuahangchay.Data;
using Cuahangchay.Models; // hoặc namespace thật

public class MonChayController : Controller
{
    private readonly ApplicationDbContext _context;

    public MonChayController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var list = _context.MonChay.ToList(); // Gọi thử dữ liệu
        return View(list);
    }
}
