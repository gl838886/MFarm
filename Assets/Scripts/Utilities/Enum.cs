 public enum ItemType
{
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,
    ReapableScenery
}

public enum SlotType
{
    Bag,Box,Shop
}

public enum InventoryLocation
{
    Player,Box
}

public enum PartType
{
    None,Carry ,Hoe ,Break, Water, Collect, Chop, Reap
}

public enum PartName
{
    Arm, Hair, Tool, Body
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum GridType
{
    Diggable, CanDropItem, CanPlaceFurniture, NPC_Obstacles
}

public enum ParticleEffectType
{
    None, LeaveFall01, LeaveFall02, RockFall, GrassParticle
}

public enum GameState
{
    GamePause, GamePlay
}

public enum LightShift
{
    Morning, Night
}

public enum SoundName
{
    none,FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water ,Basket, Chop,
    Pickup, Plant, TreeFalling, Rustle,
    AmbientCountrySide1, AmbientCountrySide2, MusicCalm1, MusicCalm3, AmbientInDoor1
}