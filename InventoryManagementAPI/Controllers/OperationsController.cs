using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementAPI.Data;
using InventoryManagementAPI.Models;

namespace InventoryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Operations (عرض جميع عمليات حركة المخزون)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Operation>>> GetOperations()
        {
            return await _context.Operations
                .Include(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Supplier)
                .ToListAsync();
        }

        // GET: api/Operations/5 (عرض تفاصيل عملية محدودة)
        [HttpGet("{id}")]
        public async Task<ActionResult<Operation>> GetOperation(int id)
        {
            var operation = await _context.Operations
                .Include(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Supplier)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (operation == null)
            {
                return NotFound("العملية غير موجودة.");
            }

            return operation;
        }

        // POST: api/Operations (تسجيل عملية صادر/وارد وتحديث كمية المخزون تلقائيًا)
        [HttpPost]
        public async Task<ActionResult<Operation>> PostOperation(Operation operation)
        {
            // 1. التحقق من وجود المنتج
            var product = await _context.Products.FindAsync(operation.ProductId);
            if (product == null)
            {
                return BadRequest("المنتج المحدد غير موجود.");
            }

            // 2. التحقق من وجود المستخدم
            var userExists = await _context.Users.AnyAsync(u => u.Id == operation.UserId);
            if (!userExists)
            {
                return BadRequest("المستخدم المحدد غير موجود.");
            }

            // 3. التحقق من وجود المورد (إذا تم تحديده)
            if (operation.SupplierId.HasValue)
            {
                var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == operation.SupplierId.Value);
                if (!supplierExists)
                {
                    return BadRequest("المورد المحدد غير موجود.");
                }
            }

            // 4. تحديث كمية المنتج في المخزن بناءً على نوع العملية
            if (operation.OperationType.Equals("In", StringComparison.OrdinalIgnoreCase))
            {
                // وارد (إضافة للمخزون)
                product.StockQuantity += operation.Quantity;
            }
            else if (operation.OperationType.Equals("Out", StringComparison.OrdinalIgnoreCase))
            {
                // صادر (سحب من المخزون)
                if (product.StockQuantity < operation.Quantity)
                {
                    return BadRequest($"الكمية المطلوبة غير متوفرة بالكامل. المخزون الحالي: {product.StockQuantity}");
                }
                product.StockQuantity -= operation.Quantity;
            }
            else
            {
                return BadRequest("نوع العملية غير صحيح. استخدم 'In' للوارد أو 'Out' للصادر.");
            }

            operation.Date = DateTime.UtcNow;

            _context.Operations.Add(operation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOperation), new { id = operation.Id }, operation);
        }
    }
}