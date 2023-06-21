using System;
using System.Linq;
using System.Text.Json.Serialization;
using Functions;
using JSONData;

namespace Models {
  public enum ResultType{
    draw = 0,
    white = 1,
    black = 2
  }

  public class Settings {
    public int BaseModifier { get; set; }
    public int BonusModifier { get; set; }
    public int AmountModifier { get; set; }
    public int TimeModifier { get; set; }
    public int TruncateLength { get; set; }

    [JsonConstructor]
    public Settings(int baseModifier, int bonusModifier, int amountModifier, int timeModifier, int truncateLength) {
      BaseModifier = baseModifier;
      BonusModifier = bonusModifier;
      AmountModifier = amountModifier;
      TimeModifier = timeModifier;
      TruncateLength = truncateLength;
    }
  }
  
  public class Player {
    //field
    public string Name { get; set; }
    public Guid Id { get; set; }
    public string? PrefComp { get; set; }
    public string? BonusComp { get; set; }

    //constructor
    [JsonConstructor]
    public Player(string name, Guid id, string? prefComp, string? bonusComp) {
      Name = name;
      Id = id;
      PrefComp = prefComp;
      BonusComp = bonusComp;
    }
    public Player(string name, string prefComp) {
      Name = name;
      Id = Guid.NewGuid();
      PrefComp = prefComp;
    }
    public Player(string name, string prefComp, string bonusComp) {
      Name = name;
      Id = Guid.NewGuid();
      PrefComp = prefComp;
      BonusComp = bonusComp;
    }

    //methods
  }

  public class Match {
    public Guid White { get; set; }
    public Guid Black { get; set; }
    public ResultType Result { get; set; }
    public DateTime Date { get; set; }
    public int Id { get; set; }

    [JsonConstructor]
    public Match(Guid white, Guid black, ResultType result, DateTime date, int id) {
      White = white;
      Black = black;
      Result = result;
      Date = date;
      Id = id;
    }
    public Match(Player white, Player black, ResultType result, DateTime date, int id) {
      White = white.Id;
      Black = black.Id;
      Result = result;
      Date = date;
      Id = id;
    }
    public Match(Player white, Player black, ResultType result, int id) {
      White = white.Id;
      Black = black.Id;
      Result = result;
      Date = DateTime.Today;
      Id = id;
    }
  }

  public class Competition {
    public string Name { get; set; }
    public List<Match> Matches { get; set; }
    public int NextId { get; set; }
    public bool Active { get; set; }

    public Competition(string name) {
      Name = name;
      Matches = new List<Match>();
      NextId = 1;
      Active = true;
    }
    [JsonConstructor]
    public Competition(string name, List<Match> matches, int nextId, bool active) {
      Name = name;
      Matches = matches;
      NextId = nextId;
      Active = active;
    }
    public Competition(string name, List<Match> matches) {
      Name = name;
      Matches = matches;
      NextId = 1;
      Active = true;
    }

    public int GetNextId() {
      return NextId++;
    }

    public void AddMatch(Match match) {
      Matches.Add(match);
    }

    public Dictionary<Guid, int> GetPlayers() {
      Dictionary<Guid, int> players = new Dictionary<Guid, int>{};
      foreach (Match match in Matches) {
        if (players.ContainsKey(match.White)) {
          players[match.White] += 1;
        }
        else {
          players.Add(match.White, 1);
        }
        if (players.ContainsKey(match.Black)) {
          players[match.Black] += 1;
        }
        else {
          players.Add(match.Black, 1);
        }
      }
      return players;
    }

    public int GetPlayerCount() {
      return GetPlayers().Keys.Count();
    }

    public double GetMatchResult(Guid p1, Guid p2) {
      if (Matches.Where(x => ((x.White == p1 && x.Black == p2) || (x.Black == p1 && x.White == p2))).Count() == 0) {
        return -1;
      }
      return (double)Matches.Where(x => ((x.White == p1 && x.Black == p2) && x.Result == ResultType.white)).Count() + 
             (double)Matches.Where(x => ((x.Black == p1 && x.White == p2) && x.Result == ResultType.black)).Count() + 
             (double)Matches.Where(x => ((x.White == p1 && x.Black == p2) || (x.Black == p1 && x.White == p2)) && x.Result == ResultType.draw).Count() / 2;
    }

    public double GetPoints(Guid p) {
      return (double)Matches.Where(x => x.White == p && x.Result == ResultType.white).Count() +
             (double)Matches.Where(x => x.Black == p && x.Result == ResultType.black).Count() +
             (double)Matches.Where(x => (x.White == p || x.Black == p) && x.Result == ResultType.draw).Count() / 2;
    }

    public string Standings() {
      string result = Mathf.Truncate(" ", JSON.LoadSettings().TruncateLength);
      foreach (Guid p1 in GetPlayers().Keys) {
        Player? p = Mathf.FindGuid(p1);
        string txt = "###";
        if (p != null) txt = Mathf.Truncate(p.Name, 3);
        result += $"│ {txt} ";
      }
      result += "\n";

      int i = 1;
      foreach (Guid p1 in GetPlayers().Keys) {
        Player? ptemp = Mathf.FindGuid(p1);
        if (ptemp == null) {
          continue;
        }
        result += Mathf.Truncate(ptemp.Name, JSON.LoadSettings().TruncateLength);
        foreach (Guid p2 in GetPlayers().Keys) {
          string txt = "   ";
          if (p1 == p2) {
            txt = "X  ";
          }
          else if (GetMatchResult(p1, p2) != -1) {
            txt = Mathf.Truncate(GetMatchResult(p1, p2).ToString(), 3);
          }
          result += "│ " + txt + " ";
        }
        result += "\n";
        i++;
      }
      return result;
    }

    public string Leaderboard() {
      Dictionary<string, double> dict = new Dictionary<string, double>{};
      foreach (Guid p in GetPlayers().Keys) {
        Player? ptemp = Mathf.FindGuid(p);
        if (ptemp == null) {
          continue;
        }
        dict.Add(Mathf.Truncate(ptemp.Name, JSON.LoadSettings().TruncateLength), GetPoints(p));
      }
      var sortedDict = from entry in dict orderby entry.Value descending select entry;

      string result = "";
      int i = 1;
      foreach (KeyValuePair<string, double> p in sortedDict) {
        result += Mathf.Truncate(i.ToString(), 2) + " " + p.Key + " " + p.Value + "\n";
        i++;
      }
      return result;
    }
  }
}
