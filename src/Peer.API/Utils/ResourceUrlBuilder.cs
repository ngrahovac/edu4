namespace Peer.API.Utils;

public static class ResourceUrlBuilder
{
    public static string BuildProjectUrl(Guid projectId)
        => $"/projects/{projectId}";

    public static string BuildPositionUrl(Guid projectId, Guid positionId)
        => $"/projects/{projectId}/positions/{positionId}";

    public static string BuildContributorUrl(Guid contributorId)
        => $"/contributors/{contributorId}";
    public static string BuildProjectCollaborationsUrl(Guid projectId)
            => $"/collaborations/project/{projectId}";
}
