using System.Runtime.Serialization;

namespace Backend.DataModels.Config;

public class Secrets
{ 
    public string JwtSecret { get; set; }
}