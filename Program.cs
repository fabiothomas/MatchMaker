﻿using Models;
using JSONData;

Console.ForegroundColor = ConsoleColor.Cyan;

System.Console.WriteLine("");
Console.WriteLine("> Hello, What is your command? (type 'help' for a list of commands)");

bool cont = true;
while (cont == true) {
  Console.ForegroundColor = ConsoleColor.Yellow;
  string? cmd = Console.ReadLine();
  Console.ForegroundColor = ConsoleColor.Cyan;
  System.Console.WriteLine("");

  if (cmd != null) {
    string[] cmdargs = cmd.Split(' ');
    cmdargs[0] = cmdargs[0].ToLower();

    switch(cmdargs[0]){
      case "exit":
        System.Console.WriteLine("> Goodbye");
        
        cont = false;
        break;

      case "help":
        System.Console.WriteLine("> List of commands:");
        System.Console.WriteLine("> 'exit' - Exit the program");
        System.Console.WriteLine("> 'generate [usernames]' - Generate Matches with the given players");
        System.Console.WriteLine("-------------------------------------------------------------------------");
        System.Console.WriteLine("> 'addmatch [username1, username2, competition, result(white, black, draw), date?] - Add match to competition");
        System.Console.WriteLine("> 'delmatch [competition, id]' - Delete match");
        System.Console.WriteLine("");
        System.Console.WriteLine("> 'addplayer [username, competition, bonuscompetition?]' - Create new player");
        System.Console.WriteLine("> 'delplayer [username]' - Delete player");
        System.Console.WriteLine("");
        System.Console.WriteLine("> 'addcomp [name]' - Create new competition");
        System.Console.WriteLine("> 'delcomp [name]' - Delete competition");
        System.Console.WriteLine("> 'activate [competition]' - Activate a competition");
        System.Console.WriteLine("> 'deactivate [competition]' - Deactivate a competition");
        System.Console.WriteLine("-------------------------------------------------------------------------");
        System.Console.WriteLine("> 'standings [competition]' - Show a competitions standings");
        System.Console.WriteLine("> 'list [competition]' - Lists all matches in a competition");
        break;

      case "addplayer":
        if (cmdargs.Length < 3) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.AddPlayer(cmdargs);
        break;

      case "delplayer":
       if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.DelPlayer(cmdargs); 
        break;
      
      case "addcomp":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.AddComp(cmdargs);
        break;

      case "delcomp":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.DelComp(cmdargs);
        break;

      case "addmatch":
        if (cmdargs.Length < 5) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.AddMatch(cmdargs);
        break;

      case "delmatch":
        if (cmdargs.Length < 3) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.DelMatch(cmdargs);
        break;

      case "activate":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.Activate(cmdargs);
        break;

      case "deactivate":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.Deactivate(cmdargs);
        break;

      case "standings":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.Standings(cmdargs);
        break;

      case "list":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.List(cmdargs);
        break;

      case "generate":
        if (cmdargs.Length < 2) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        OBJ.Generate(cmdargs);
        break;

      case "simulate":
        if (cmdargs.Length < 1) { System.Console.WriteLine("> Missing paramaters, use 'help' for a list of commands and their paramaters"); break; }
        System.Console.WriteLine("> Starting Simulation");
        
        //settings
        JSON.SaveSettings(new Settings(0, -50, -10, 1, 12));

        //competitions
        JSON.SaveComp(new List<Competition>{
          new Competition("C"),
          new Competition("B"),
          new Competition("A")
        });

        //players
        JSON.SavePlayers(new List<Player>{
          new Player("Alessio", "C"),
          new Player("Amos", "C"),
          new Player("Ayza", "C"),
          new Player("Florian", "C"),
          new Player("Lina", "C"),
          new Player("Luca", "C"),
          new Player("Mees", "C"),
          new Player("Mihail", "C", "B"),
          new Player("Eric", "C", "B"),

          new Player("Asya", "B", "A"),
          new Player("Benjamin", "B"),
          new Player("Daan", "B"),
          new Player("Emery", "B"),
          new Player("Merlijn", "B", "C"),
          new Player("Jim", "B"),
          new Player("Noah", "B", "C"),
          new Player("Simon", "B", "A"),
          new Player("Steven", "B"),
          new Player("Ayelen", "B"),

          new Player("David", "A"),
          new Player("Ruben", "A"),
          new Player("Niek", "A"),
          new Player("Job", "A"),
          new Player("Max", "A"),
          new Player("Tariq", "A", "B"),
          new Player("Tibor", "A"),
          new Player("Daniel", "A", "B"),
        });
        
        //matches
        

        break;

      default:
        System.Console.WriteLine("> Unrecognized command, try 'help' for a list of commands");
        break;
    }
  }
}

Console.ResetColor();
