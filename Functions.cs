using JSONData;
using Models;

namespace Functions {
  public static class Mathf {
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value + new String(' ', maxLength - value.Length): value.Substring(0, maxLength); 
    }

    public static Player? FindGuid(Guid id) {
      return JSON.LoadPlayers().Where(x => x.Id == id).FirstOrDefault();
    }
  }
}