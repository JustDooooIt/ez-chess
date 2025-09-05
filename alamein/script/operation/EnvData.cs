public record EnvData : BaseData
{
  public EnvDataType Type { get; set; }
}

public record FactionData : EnvData
{
  public int Faction { get; set; }
}

public record TurnData : EnvData
{ 
  public int Turn { get; set; }
}

public enum EnvDataType
{
  FACTION, TURN
}