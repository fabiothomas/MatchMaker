using System.Text.Json;
using System.Text.Json.Serialization;
using Models;
using Functions;

namespace JSONData {
  public static class JSON {
    public static void SaveComp(List<Competition> data) {
      var options = new JsonSerializerOptions { WriteIndented = true };

      string json = JsonSerializer.Serialize(data, options);
      File.WriteAllText("./JSONData/Competitions.json", json);
    }

    public static List<Competition> LoadComp() {
      List<Competition>? data = JsonSerializer.Deserialize<List<Competition>>(File.ReadAllText("./JSONData/Competitions.json"));
      if (data != null) {
        return data;
      }
      return new List<Competition>{};
    }

    public static void SavePlayers(List<Player> data) {
      var options = new JsonSerializerOptions { WriteIndented = true };

      string json = JsonSerializer.Serialize(data, options);
      File.WriteAllText("./JSONData/Players.json", json);
    }

    public static List<Player> LoadPlayers() {
      List<Player>? data = JsonSerializer.Deserialize<List<Player>>(File.ReadAllText("./JSONData/Players.json"));
      if (data != null) {
        return data;
      }
      return new List<Player>{};
    }

    public static void SaveSettings(Settings data) {
      var options = new JsonSerializerOptions { WriteIndented = true };

      string json = JsonSerializer.Serialize(data, options);
      File.WriteAllText("./JSONData/Settings.json", json);
    }

    public static Settings LoadSettings() {
      Settings? data = JsonSerializer.Deserialize<Settings>(File.ReadAllText("./JSONData/Settings.json"));
      if (data != null) {
        return data;
      }
      Settings settings = new Settings(0, -50, -10, 1, 12);
      SaveSettings(settings);
      return settings;
    }
  }

  public static class OBJ {
    public static void AddPlayer(string[] cmdargs) {
      List<Player> list = JSON.LoadPlayers();
      if (list.Where(x => x.Name == cmdargs[1]).FirstOrDefault() == null) {
        System.Console.WriteLine($"> Created new player: {cmdargs[1]}");
        list.Add(new Player(cmdargs[1], cmdargs[2]));
        JSON.SavePlayers(list);
        return;
      }
      System.Console.WriteLine($"> {cmdargs[1]} already exists");
      return;
    }
    public static void DelPlayer(string[] cmdargs) {
      List<Player> list = JSON.LoadPlayers();
      Player? p = null;
      try {
        p = list.Where(x => x.Id == Guid.Parse(cmdargs[1])).FirstOrDefault();
      }
      catch {
        p = list.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      }
      if (p == null) {
        System.Console.WriteLine($"> Player not found");
        return;
      }
      System.Console.WriteLine($"> {p.Name} succefully removed");
      list.Remove(p);
      JSON.SavePlayers(list);
      return;
    }
    public static void AddComp(string[] cmdargs) {
      List<Competition> list = JSON.LoadComp();
      if (list.Where(x => x.Name == cmdargs[1]).FirstOrDefault() == null) {
        System.Console.WriteLine($"> Created new competition: {cmdargs[1]}");
        list.Add(new Competition(cmdargs[1]));
        JSON.SaveComp(list);
        return;
      }
      System.Console.WriteLine($"> {cmdargs[1]} already exists");
      return;
    }
    public static void DelComp(string[] cmdargs) {
      List<Competition> list = JSON.LoadComp();
      Competition? comp = list.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine("> Competition not found");
        return;
      }
      System.Console.WriteLine($"> {comp.Name} succefully removed");
      list.Remove(comp);
      JSON.SaveComp(list);
      return;
    }

    public static void AddMatch(string[] cmdargs) {
      //verify p1
      List<Player> plist = JSON.LoadPlayers();
      Player? p1 = null;
      try {
        p1 = plist.Where(x => x.Id == Guid.Parse(cmdargs[1])).FirstOrDefault();
      }
      catch {
        p1 = plist.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      }
      if (p1 == null) {
        System.Console.WriteLine($"> {cmdargs[1]} not found");
        return;
      }

      //verify p2
      Player? p2 = null;
      try {
        p2 = plist.Where(x => x.Id == Guid.Parse(cmdargs[2])).FirstOrDefault();
      }
      catch {
        p2 = plist.Where(x => x.Name == cmdargs[2]).FirstOrDefault();
      }
      if (p2 == null) {
        System.Console.WriteLine($"> {cmdargs[2]} not found");
        return;
      }

      //verify comp
      List<Competition> clist = JSON.LoadComp();
      Competition? comp = clist.Where(x => x.Name == cmdargs[3]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine("> Competition not found");
        return;
      }
      
      //verify result
      if (cmdargs[4] != "white" && cmdargs[4] != "black" && cmdargs[4] != "draw") {
        System.Console.WriteLine($"> {cmdargs[4]} is not a valid result");
        return;
      }
      ResultType result = ResultType.draw;
      if (cmdargs[4] == "white") result = ResultType.white;
      if (cmdargs[4] == "black") result = ResultType.black;

      //create match
      if (cmdargs.Length <= 5) {
        if (result != ResultType.draw) System.Console.WriteLine($"> Succesfully added match between {p1.Name} (white) and {p2.Name} (black) where {cmdargs[4]} won on {DateTime.Today}. Match id: {comp.NextId}");
        else System.Console.WriteLine($"> Succesfully added match between {p1.Name} (white) and {p2.Name} (black) where they drew on {DateTime.Today}. Match id: {comp.NextId}");
        comp.Matches.Add(new Match(p1, p2, result, comp.GetNextId())); 
        JSON.SaveComp(clist);
      }
      else {
        try {
          comp.Matches.Add(new Match(p1, p2, result, DateTime.Parse(cmdargs[5]), comp.GetNextId())); 
          JSON.SaveComp(clist);
          if (result != ResultType.draw) System.Console.WriteLine($"> Succesfully added match between {p1.Name} (white) and {p2.Name} (black) where {cmdargs[4]} won on {DateTime.Parse(cmdargs[5])}. Match id: {comp.NextId}");
          else System.Console.WriteLine($"> Succesfully added match between {p1.Name} (white) and {p2.Name} (black) where they drew on {DateTime.Parse(cmdargs[5])}. Match id: {comp.NextId}");
        }
        catch {
          System.Console.WriteLine($"> Couldn't read the date: {cmdargs[5]}");
        }
      }
    }

    public static void DelMatch(string[] cmdargs) {
      List<Competition> clist = JSON.LoadComp();
      Competition? comp = clist.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine("> Competition not found");
        return;
      }

      Match? match = comp.Matches.Where(x => x.Id == int.Parse(cmdargs[2])).FirstOrDefault();
      if (match == null) {
        System.Console.WriteLine("> Match not found");
        return;
      }
      Player? p1 = Mathf.FindGuid(match.White);
      Player? p2 = Mathf.FindGuid(match.Black);
      string txt1 = "Removed Player";
      string txt2 = "Removed Player";
      if (p1 != null) txt1 = p1.Name;
      if (p2 != null) txt2 = p2.Name;

      System.Console.WriteLine($"> Succesfully removed match between {txt1} and {txt2}");
      comp.Matches.Remove(match);
      JSON.SaveComp(clist);
    }

    public static void Activate(string[] cmdargs) {
      List<Competition> clist = JSON.LoadComp();
      Competition? comp = clist.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine("> Competition not found");
        return;
      }

      if (comp.Active == true) {
        System.Console.WriteLine($"> {cmdargs[1]} is already active");
        return;
      }

      System.Console.WriteLine($"> Succesfully activated {cmdargs[1]}");
      comp.Active = true;
      JSON.SaveComp(clist);
    }

    public static void Deactivate(string[] cmdargs) {
      List<Competition> clist = JSON.LoadComp();
      Competition? comp = clist.Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine("> Competition not found");
        return;
      }

      if (comp.Active == false) {
        System.Console.WriteLine($"> {cmdargs[1]} is already not active");
        return;
      }

      System.Console.WriteLine($"> Succesfully deactivated {cmdargs[1]}");
      comp.Active = false;
      JSON.SaveComp(clist);
    }

    public static void Standings(string[] cmdargs) {
      Competition? comp = JSON.LoadComp().Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine($"> Competition {cmdargs[1]} not found");
        return;
      }
      System.Console.WriteLine(comp.Standings());
      System.Console.WriteLine(comp.Leaderboard());
      return;
    }

    public static void List(string[] cmdargs) {
      Competition? comp = JSON.LoadComp().Where(x => x.Name == cmdargs[1]).FirstOrDefault();
      if (comp == null) {
        System.Console.WriteLine($"> Competition {cmdargs[1]} not found");
        return;
      }
      foreach (Match match in comp.Matches) {
        Player? p1 = Mathf.FindGuid(match.White);
        Player? p2 = Mathf.FindGuid(match.Black);
        string txt1 = "Removed Player";
        string txt2 = "Removed Player";
        if (p1 != null) txt1 = p1.Name;
        if (p2 != null) txt2 = p2.Name;
        string txt3 = "";
        if (match.Result == ResultType.white) txt3 = "1-0";
        if (match.Result == ResultType.black) txt3 = "0-1";
        if (match.Result == ResultType.draw) txt3 = "½-½";
        System.Console.WriteLine($"- {Mathf.Truncate(match.Id.ToString(), 2)}: {Mathf.Truncate(txt1, JSON.LoadSettings().TruncateLength)} vs {Mathf.Truncate(txt2, JSON.LoadSettings().TruncateLength)}  {Mathf.Truncate(txt3, 3)}  {match.Date}");
      }
    }

    public static void Generate(string[] cmdargs) {
      //verify competitions
      List<Competition> comps = JSON.LoadComp().Where(x => x.Active == true).ToList();
      if (comps.Count() == 0) {
        System.Console.WriteLine($"> [Warning] No active competitions found not found");
      }
      else {
        System.Console.WriteLine("> Active competitions:");
        foreach (Competition comp in comps) {
          System.Console.WriteLine($"- {comp.Name}, with {comp.Matches.Count()} matches");
        }
      }
      System.Console.WriteLine("");

      //create all combinations
      List<Player> list = JSON.LoadPlayers();
      Dictionary<Tuple<Player, Player>, int> result = new Dictionary<Tuple<Player, Player>, int>{};
      for (int i = 1; i < cmdargs.Length; i++) {
        string name = cmdargs[i];
        Player? p1 = list.Where(x => x.Name == name).FirstOrDefault();
        if (p1 == null) {
          System.Console.WriteLine($"> {name} not found");
          return;
        }

        foreach (string name2 in cmdargs.Skip(i+1).ToArray()) {
          Player? p2 = list.Where(x => x.Name == name2).FirstOrDefault();
          if (p2 == null) {
            System.Console.WriteLine($"> {name2} not found");
            return;
          }
          if (p1.PrefComp == p2.PrefComp) {
            result.Add(new Tuple<Player, Player>(p1, p2), JSON.LoadSettings().BaseModifier);
          }
          else if ((p1.BonusComp == p2.PrefComp && p1.PrefComp == p2.BonusComp) || (p1.BonusComp == p2.PrefComp && p2.BonusComp == null) || (p1.PrefComp == p2.BonusComp && p1.BonusComp == null)) {
            //if (!(p1.BonusComp != null && p2.BonusComp != null && p1.BonusComp != p2.BonusComp)) {
            result.Add(new Tuple<Player, Player>(p1, p2), JSON.LoadSettings().BaseModifier + JSON.LoadSettings().BonusModifier);
          }
        }
      }

      //score all combinations
      foreach (Tuple<Player, Player> players in result.Keys) {
        foreach (Competition comp in comps) {
          List<Match> matches = comp.Matches.Where(x => (x.White == players.Item1.Id && x.Black == players.Item2.Id) || (x.White == players.Item2.Id && x.Black == players.Item1.Id)).OrderBy(x => (DateTime.Today - x.Date).Days).ToList();

          result[players] += matches.Count() * JSON.LoadSettings().AmountModifier;
          if (matches.Count() != 0) result[players] += (DateTime.Today - matches[0].Date).Days / 7 * JSON.LoadSettings().TimeModifier;
        }
      }

      List<Tuple<Player, Player>> ordered = result.Keys.OrderByDescending(x => result[x]).ToList();
      foreach (var test in ordered) {
        System.Console.WriteLine($"test {Mathf.Truncate(test.Item1.Name, JSON.LoadSettings().TruncateLength)} vs {Mathf.Truncate(test.Item2.Name, JSON.LoadSettings().TruncateLength)} : {result[test]}");
      }

      //prune results
      List<Guid> reserved = new List<Guid>();
      foreach (Tuple<Player, Player> pair in new List<Tuple<Player, Player>>(ordered)) {
        if (reserved.Contains(pair.Item1.Id) || reserved.Contains(pair.Item2.Id)) {
          ordered.Remove(pair);
        }
        else {
          reserved.Add(pair.Item1.Id);
          reserved.Add(pair.Item2.Id);
        }
      }

      //print result
      System.Console.WriteLine("> Generated matches:");
      foreach (Tuple<Player, Player> players in ordered) {
        System.Console.WriteLine($"- {Mathf.Truncate(players.Item1.Name, JSON.LoadSettings().TruncateLength)} vs {Mathf.Truncate(players.Item2.Name, JSON.LoadSettings().TruncateLength)} : {result[players]}");
      }
    }
  }
}