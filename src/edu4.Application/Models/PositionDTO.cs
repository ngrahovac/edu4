using edu4.Domain.Projects;

namespace edu4.Application.Models;

public record PositionDTO(string Name, string Description, HatDTO Requirements)
{
    public static Position ToPosition(PositionDTO model)
    {
        return new Position(
            model.Name,
            model.Description,
            HatDTO.ToHat(model.Requirements));
    }
}
