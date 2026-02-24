using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class ChangeStatusRequest
{
    public ListcaseStatus NewStatus { get; set; }
}