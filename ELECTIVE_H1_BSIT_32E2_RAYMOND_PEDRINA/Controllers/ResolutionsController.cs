using Microsoft.AspNetCore.Mvc;
using ELECTIVE_H1_BSIT_32E2_RAYMOND_PEDRINA.Models;

namespace ELECTIVE_H1_BSIT_32E2_RAYMOND_PEDRINA.Controllers;

[ApiController]
[Route("api/resolutions")]
public class ResolutionsController : ControllerBase
{
    private static readonly List<Resolution> Resolutions = new();
    private static int NextId = 1;

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? isDone, [FromQuery] string? title)
    {
        var items = Resolutions.AsEnumerable();

        if (!string.IsNullOrEmpty(isDone))
        {
            if (!bool.TryParse(isDone, out bool done))
            {
                return BadRequest(new
                {
                    error = "BadRequest",
                    message = "Validation failed.",
                    details = new[] { "isDone must be true or false" }
                });
            }

            items = items.Where(r => r.IsDone == done);
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            items = items.Where(r =>
                r.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        var result = items.Select(r =>
        {
            var obj = new Dictionary<string, object?>
            {
                ["id"] = r.Id,
                ["title"] = r.Title,
                ["isDone"] = r.IsDone,
                ["createdAt"] = r.CreatedAt
            };
            if (r.UpdatedAt.HasValue)
                obj["updatedAt"] = r.UpdatedAt.Value;

            return obj;
        });

        return Ok(new { items = result });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "id must be greater than zero" }
            });
        }

        var res = Resolutions.FirstOrDefault(r => r.Id == id);
        if (res == null)
        {
            return NotFound(new
            {
                error = "NotFound",
                message = "Resource not found.",
                details = new[] { $"resolution with id {id} not found" }
            });
        }

        var response = new Dictionary<string, object?>
        {
            ["id"] = res.Id,
            ["title"] = res.Title,
            ["isDone"] = res.IsDone,
            ["createdAt"] = res.CreatedAt
        };

        if (res.UpdatedAt.HasValue)
            response["updatedAt"] = res.UpdatedAt.Value;

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Resolution? body)
    {
        if (body == null || string.IsNullOrWhiteSpace(body.Title))
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "title is required" }
            });
        }

        var resolution = new Resolution
        {
            Id = NextId++,
            Title = body.Title,
            IsDone = false,
            CreatedAt = DateTime.Now
        };

        Resolutions.Add(resolution);

        return CreatedAtAction(nameof(GetById), new { id = resolution.Id }, new
        {
            resolution.Id,
            resolution.Title,
            resolution.IsDone,
            resolution.CreatedAt
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Resolution? body)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "route id must be greater than zero" }
            });
        }

        if (body == null || body.Id == 0)
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "body id is required" }
            });
        }

        if (id != body.Id)
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Route id does not match body id.",
                details = new[]
                {
                    $"route id: {id}",
                    $"body id: {body.Id}"
                }
            });
        }

        if (string.IsNullOrWhiteSpace(body.Title))
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "title is required" }
            });
        }

        var existing = Resolutions.FirstOrDefault(r => r.Id == id);
        if (existing == null)
        {
            return NotFound(new
            {
                error = "NotFound",
                message = "Resource not found.",
                details = new[] { $"resolution with id {id} not found" }
            });
        }

        existing.Title = body.Title;
        existing.IsDone = body.IsDone;
        existing.UpdatedAt = DateTime.Now;

        return Ok(new
        {
            existing.Id,
            existing.Title,
            existing.IsDone,
            updatedAt = existing.UpdatedAt
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                error = "BadRequest",
                message = "Validation failed.",
                details = new[] { "id must be greater than zero" }
            });
        }

        var res = Resolutions.FirstOrDefault(r => r.Id == id);
        if (res == null)
        {
            return NotFound(new
            {
                error = "NotFound",
                message = "Resource not found.",
                details = new[] { $"resolution with id {id} not found" }
            });
        }

        Resolutions.Remove(res);
        return NoContent();
    }
}