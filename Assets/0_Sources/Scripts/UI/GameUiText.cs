using UnityEngine;
using YG;

public static class GameUiText
{
    public static string Pick(string ru, string en, string tr = null)
    {
        var lang = YG2.lang;
        if (lang == "ru")
            return ru;
        if (lang == "tr" && string.IsNullOrEmpty(tr) == false)
            return tr;
        return en;
    }

    public static string FormatClock(int seconds)
    {
        seconds = Mathf.Max(0, seconds);
        int minutes = seconds / 60;
        int rest = seconds % 60;
        return $"{minutes}:{rest:00}";
    }

    public static string FormatDuration(int seconds)
    {
        seconds = Mathf.Max(0, seconds);
        if (seconds < 60)
            return Pick($"{seconds} сек", $"{seconds} sec", $"{seconds} sn");

        int minutes = seconds / 60;
        int rest = seconds % 60;
        if (rest == 0)
            return Pick($"{minutes} мин", $"{minutes} min", $"{minutes} dk");

        return Pick($"{minutes} мин {rest} сек", $"{minutes} min {rest} sec", $"{minutes} dk {rest} sn");
    }
}
