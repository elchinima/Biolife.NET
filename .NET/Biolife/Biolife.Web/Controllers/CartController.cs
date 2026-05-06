public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly UserService _userService;
    private readonly OrderService _orderService;

    public CartController(CartService cartService, UserService userService, OrderService orderService)
    {
        _cartService = cartService;
        _userService = userService;
        _orderService = orderService;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var items = await _cartService.GetItemsAsync(userId.Value);
        if (items.Count == 0)
            return BadRequest(new { message = "Your cart is empty." });

        var (success, error, user) = await _userService.ValidateCheckoutProfileAsync(userId.Value);
        if (!success || user is null)
            return BadRequest(new { message = error });

        var order = await _orderService.CreateFromCartAsync(userId.Value);
        if (order is null)
            return BadRequest(new { message = "Could not place the order. Please refresh the page and try again." });

        return Json(new
        {
            message = $"Order #{order.Id} placed successfully. Thank you for shopping with Biolife.",
            items = Array.Empty<CartItemVm>()
        });
    }

    private int? GetCurrentUserId()
    {
        var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idValue, out var userId) ? userId : null;
    }
}


