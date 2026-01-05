using Microsoft.AspNetCore.Mvc;
using DashboardWolverine.Repositories;
using DashboardWolverine.Models;

namespace DashboardWolverine.Controllers;

[ApiController]
[Route("api/wolverine")]
public class WolverineController : ControllerBase
{
    private readonly WolverineRepository _repository;
    private readonly ILogger<WolverineController> _logger;

    public WolverineController(WolverineRepository repository, ILogger<WolverineController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    #region Dashboard Stats

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _repository.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Dead Letters Endpoints

    [HttpGet("dead-letters")]
    public async Task<IActionResult> GetAllDeadLetters(
        [FromQuery] string? messageType = null,
        [FromQuery] string? exceptionType = null,
        [FromQuery] string? bodySearch = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _repository.GetAllDeadLettersAsync(
                messageType, exceptionType, bodySearch, startDate, endDate, page, pageSize);
            
            return Ok(new { 
                count = result.TotalCount,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                data = result.Data,
                filters = new {
                    messageTypes = result.AvailableMessageTypes,
                    exceptionTypes = result.AvailableExceptionTypes
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dead letters");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("dead-letters/{id}")]
    public async Task<IActionResult> GetDeadLetterById(Guid id, [FromQuery] string receivedAt)
    {
        try
        {
            if (string.IsNullOrEmpty(receivedAt))
            {
                return BadRequest(new { error = "receivedAt query parameter is required" });
            }

            var deadLetter = await _repository.GetDeadLetterByIdAsync(id, receivedAt);
            
            if (deadLetter == null)
            {
                return NotFound(new { error = "Dead letter not found" });
            }

            return Ok(deadLetter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dead letter by id");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("dead-letters/{id}/replay")]
    public async Task<IActionResult> SetDeadLetterReplayable(Guid id, [FromQuery] string receivedAt, [FromBody] ReplayRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(receivedAt))
            {
                return BadRequest(new { error = "receivedAt query parameter is required" });
            }

            var updated = await _repository.SetDeadLetterReplayableAsync(id, receivedAt, request.Replayable);
            
            if (updated == 0)
            {
                return NotFound(new { error = "Dead letter not found" });
            }

            return Ok(new { message = "Dead letter updated successfully", replayable = request.Replayable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dead letter replayable status");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("dead-letters/replay-multiple")]
    public async Task<IActionResult> SetMultipleDeadLettersReplayable([FromBody] BulkReplayRequest request)
    {
        try
        {
            if (request.DeadLetters == null || request.DeadLetters.Count == 0)
            {
                return BadRequest(new { error = "DeadLetters list cannot be empty" });
            }

            var deadLetters = request.DeadLetters.Select(dl => (dl.Id, dl.ReceivedAt)).ToList();
            var updated = await _repository.SetMultipleDeadLettersReplayableAsync(deadLetters, request.Replayable);

            return Ok(new { message = $"Updated {updated} dead letters successfully", count = updated, replayable = request.Replayable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating multiple dead letters");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("dead-letters/{id}")]
    public async Task<IActionResult> DeleteDeadLetter(Guid id, [FromQuery] string receivedAt)
    {
        try
        {
            if (string.IsNullOrEmpty(receivedAt))
            {
                return BadRequest(new { error = "receivedAt query parameter is required" });
            }

            var deleted = await _repository.DeleteDeadLetterAsync(id, receivedAt);
            
            if (deleted == 0)
            {
                return NotFound(new { error = "Dead letter not found" });
            }

            return Ok(new { message = "Dead letter deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dead letter");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Incoming Envelopes Endpoints

    [HttpGet("incoming-envelopes")]
    public async Task<IActionResult> GetAllIncomingEnvelopes(
        [FromQuery] string? messageType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? bodySearch = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _repository.GetAllIncomingEnvelopesAsync(
                messageType, status, bodySearch, startDate, endDate, page, pageSize);
            
            return Ok(new { 
                count = result.TotalCount,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                data = result.Data,
                filters = new {
                    messageTypes = result.AvailableMessageTypes,
                    statuses = result.AvailableStatuses
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incoming envelopes");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("incoming-envelopes/{id}")]
    public async Task<IActionResult> GetIncomingEnvelopeById(Guid id, [FromQuery] string receivedAt)
    {
        try
        {
            if (string.IsNullOrEmpty(receivedAt))
            {
                return BadRequest(new { error = "receivedAt query parameter is required" });
            }

            var envelope = await _repository.GetIncomingEnvelopeByIdAsync(id, receivedAt);
            
            if (envelope == null)
            {
                return NotFound(new { error = "Incoming envelope not found" });
            }

            return Ok(envelope);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting incoming envelope by id");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("incoming-envelopes/{id}")]
    public async Task<IActionResult> DeleteIncomingEnvelope(Guid id, [FromQuery] string receivedAt)
    {
        try
        {
            if (string.IsNullOrEmpty(receivedAt))
            {
                return BadRequest(new { error = "receivedAt query parameter is required" });
            }

            var deleted = await _repository.DeleteIncomingEnvelopeAsync(id, receivedAt);
            
            if (deleted == 0)
            {
                return NotFound(new { error = "Incoming envelope not found" });
            }

            return Ok(new { message = "Incoming envelope deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting incoming envelope");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Nodes Endpoints

    [HttpGet("nodes")]
    public async Task<IActionResult> GetAllNodes()
    {
        try
        {
            var nodes = await _repository.GetAllNodesAsync();
            return Ok(new { count = nodes.Count, data = nodes });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting nodes");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("nodes/{id}")]
    public async Task<IActionResult> GetNodeById(Guid id)
    {
        try
        {
            var node = await _repository.GetNodeByIdAsync(id);
            
            if (node == null)
            {
                return NotFound(new { error = "Node not found" });
            }

            return Ok(node);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting node by id");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("nodes/{id}")]
    public async Task<IActionResult> DeleteNode(Guid id)
    {
        try
        {
            var deleted = await _repository.DeleteNodeAsync(id);
            
            if (deleted == 0)
            {
                return NotFound(new { error = "Node not found" });
            }

            return Ok(new { message = "Node deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting node");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion

    #region Node Assignments Endpoints

    [HttpGet("node-assignments")]
    public async Task<IActionResult> GetAllNodeAssignments()
    {
        try
        {
            var assignments = await _repository.GetAllNodeAssignmentsAsync();
            return Ok(new { count = assignments.Count, data = assignments });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting node assignments");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("node-assignments/{id}")]
    public async Task<IActionResult> GetNodeAssignmentById(string id)
    {
        try
        {
            var assignment = await _repository.GetNodeAssignmentByIdAsync(id);
            
            if (assignment == null)
            {
                return NotFound(new { error = "Node assignment not found" });
            }

            return Ok(assignment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting node assignment by id");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("node-assignments/{id}")]
    public async Task<IActionResult> DeleteNodeAssignment(string id)
    {
        try
        {
            var deleted = await _repository.DeleteNodeAssignmentAsync(id);
            
            if (deleted == 0)
            {
                return NotFound(new { error = "Node assignment not found" });
            }

            return Ok(new { message = "Node assignment deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting node assignment");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    #endregion
}

#region Request DTOs

public class ReplayRequest
{
    public bool Replayable { get; set; }
}

public class BulkReplayRequest
{
    public List<DeadLetterIdentifier> DeadLetters { get; set; } = new();
    public bool Replayable { get; set; }
}

public class DeadLetterIdentifier
{
    public Guid Id { get; set; }
    public string ReceivedAt { get; set; } = string.Empty;
}

#endregion
