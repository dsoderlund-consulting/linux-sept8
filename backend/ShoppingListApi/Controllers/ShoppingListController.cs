using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingListApi.Data;
using ShoppingListApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListApi.Controllers
{
    [Route("api/items")] // Base route for this controller
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly ShoppingListContext _context;

        public ShoppingListController(ShoppingListContext context)
        {
            _context = context;
        }

        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingListItem>>> GetShoppingListItems()
        {
            // Handle case where DbSet might be null if context isn't configured correctly
            if (_context.ShoppingListItems == null)
            {
                return NotFound("Shopping list context not available.");
            }
            return await _context.ShoppingListItems.ToListAsync();
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingListItem>> GetShoppingListItem(int id)
        {
             if (_context.ShoppingListItems == null)
             {
                 return NotFound("Shopping list context not available.");
             }
            var shoppingListItem = await _context.ShoppingListItems.FindAsync(id);

            if (shoppingListItem == null)
            {
                return NotFound();
            }

            return shoppingListItem;
        }

        // POST: api/items
        [HttpPost]
        public async Task<ActionResult<ShoppingListItem>> PostShoppingListItem(ShoppingListItem shoppingListItem)
        {
             if (_context.ShoppingListItems == null)
             {
                 return Problem("Entity set 'ShoppingListContext.ShoppingListItems' is null.");
             }

             // Simple validation example
             if (string.IsNullOrWhiteSpace(shoppingListItem.Description))
             {
                return BadRequest("Item description cannot be empty.");
             }

            // Ensure ID is not set by client on creation and IsDone defaults to false
            var newItem = new ShoppingListItem
            {
                Description = shoppingListItem.Description,
                IsDone = false // Default new items to not done
            };


            _context.ShoppingListItems.Add(newItem);
            await _context.SaveChangesAsync();

            // Return the created item with its generated ID
            return CreatedAtAction(nameof(GetShoppingListItem), new { id = newItem.Id }, newItem);
        }

        // PUT: api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingListItem(int id, ShoppingListItem shoppingListItem)
        {
            if (id != shoppingListItem.Id)
            {
                return BadRequest("ID mismatch between route parameter and item body.");
            }

            // Simple validation example
             if (string.IsNullOrWhiteSpace(shoppingListItem.Description))
             {
                return BadRequest("Item description cannot be empty.");
             }


            _context.Entry(shoppingListItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShoppingListItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw other concurrency issues
                }
            }

            return NoContent(); // Standard response for successful PUT with no body content needed
        }

        // DELETE: api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingListItem(int id)
        {
            if (_context.ShoppingListItems == null)
            {
               return NotFound("Shopping list context not available.");
            }
            var shoppingListItem = await _context.ShoppingListItems.FindAsync(id);
            if (shoppingListItem == null)
            {
                return NotFound();
            }

            _context.ShoppingListItems.Remove(shoppingListItem);
            await _context.SaveChangesAsync();

            return NoContent(); // Standard response for successful DELETE
        }

        private bool ShoppingListItemExists(int id)
        {
            // Use AnyAsync for efficiency, ensure _context.ShoppingListItems is not null
            return (_context.ShoppingListItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}