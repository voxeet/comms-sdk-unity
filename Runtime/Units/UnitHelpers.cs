using System.Collections.Generic;

namespace DolbyIO.Comms.Unity
{
    static class UnitHelpers
    {
        public static Dictionary<string, string> Tips = new Dictionary<string, string>()
        {
            {"AccessToken", "The access token provided by the customer's backend."},
            {"ParticipantName", "The name of the participant."},
            {"ParticipantId", "The ID of the participant."},
            {"SpatialAudio", "A boolean that enables spatial audio for the joining participant. This boolean must be set to true if spatial audio style is enabled. For more information, refer to our sample application code."},
            {"Scale", "A scale that defines how to convert units from the coordinate system of an application (pixels or centimeters) into meters used by the spatial audio coordinate system."},
            {"Forward", "A vector describing the direction the application considers as forward. The value can be either +1, 0, or -1 and must be orthogonal to up and right."},
            {"Up", "A vector describing the direction the application considers as up. The value can be either +1, 0, or -1 and must be orthogonal to forward and right."},
            {"Right", "A vector describing the direction the application considers as right. The value can be either +1, 0, or -1 and must be orthogonal to forward and up."},
            {"Position", "The position of the participant."},
            {"Direction", "The direction of the participant."}
        };
    }
}