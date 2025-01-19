namespace SpeechDiscordBot.Models.Loldle;

public sealed record Champion
{
    public string Name { get; init; }
    public string Title { get; init; }
    public string PartType { get; init; }
    public Gender Gender { get; init; }
    public Position[] Position { get; init; }
    public RangeType Range { get; init; }// if range >= 350
    public Region Region { get; init; }
    public int YearRelease { get; init; }

    // public ChampionStatistics Statistics { get; init; }
    //Specles
}

public enum Gender
{
    Male,
    Female
}

public enum Position
{
    Top,
    Jungle,
    Middle,
    Bottom,
    Support
}

public enum RangeType
{
    Range,
    Mele
}

public enum Region
{
    Freljord,
    Shurima,
}

// public sealed record ChampionStatistics
// {
//     public int HealthPoints { get; init; }
//     public int ManaPool { get; init; }
//     public int Armor { get; init; }
//     public int AttackRange { get; init; }
//     public int AttackDamage { get; init; }
//     public int AttackSpeed { get; init; }
//     // "stats": {
//     //     "hpperlevel": 104,
//     //     "mpperlevel": 25,
//     //     "movespeed": 330,
//     //     "armorperlevel": 4.7,
//     //     "spellblock": 30,
//     //     "spellblockperlevel": 1.3,
//     //     "hpregen": 2.5,
//     //     "hpregenperlevel": 0.6,
//     //     "mpregen": 8,
//     //     "mpregenperlevel": 0.8,
//     //     "crit": 0,
//     //     "critperlevel": 0,
//     //     "attackdamageperlevel": 3,
//     //     "attackspeedperlevel": 2.2,
//     // }
// }

//Pick a champ
//Get all
//Select one random
//Store the random on db
//if next day is same as db re-roll
//after week remove the data in db


//We have a champ and now we need the info about it