using CentaurScores.Model;
using CentaurScores.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentaurScores;

public static class ModelExtensions
{
    public static ListConfigurationModel GetConfiguration(this ParticipantListEntity participantListEntity)
    {
        return string.IsNullOrWhiteSpace(participantListEntity.ConfigurationJSON)
            ? ListConfigurationModel.Default
            : System.Text.Json.JsonSerializer.Deserialize<ListConfigurationModel>(participantListEntity.ConfigurationJSON) ?? ListConfigurationModel.Default;
    }
}
