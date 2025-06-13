using EntranceManager.Models.Mappers;

public class FeeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public EntranceSummaryDto Entrance { get; set; }
    public List<ApartmentFeeDto> ApartmentFees { get; set; }
}