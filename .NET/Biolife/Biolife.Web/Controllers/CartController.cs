public class CartController : Controller
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Items()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Json(new { isAuthenticated = false, items = Array.Empty<CartItemVm>() });

        var items = await _cartService.GetItemsAsync(userId.Value);
        return Json(new { isAuthenticated = true, items });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CartItemInputVm input)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.AddAsync(userId.Value, input);
        return Json(new { isAuthenticated = true, items });
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update(int id, [FromBody] CartQuantityInputVm input)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.UpdateAsync(userId.Value, id, input.Quantity);
        if (items is null)
            return NotFound();

        return Json(new { isAuthenticated = true, items });
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.RemoveAsync(userId.Value, id);
        if (items is null)
            return NotFound();

        return Json(new { isAuthenticated = true, items });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.ClearAsync(userId.Value);
        return Json(new { isAuthenticated = true, items });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> MergeLocal([FromBody] CartMergeInputVm input)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.MergeAsync(userId.Value, input.Items);
        return Json(new { isAuthenticated = true, items });
    }

    private int? GetCurrentUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idValue, out var userId) ? userId : null;
    }
}
