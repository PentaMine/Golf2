using System;
using System.IO;
using UnityEngine;

public class Settings
{
    public enum Language
    {
        ENGLISH,
        CROATIAN
    }

    public static Language language = Language.ENGLISH;

    public static void SaveSettings()
    {
        // save 1 if the language is set to eng or 0 if it is set to cro
        File.WriteAllText("./settings.txt", language == Language.ENGLISH ? "1" : "0");
    }
    
    public static void LoadSettings()
    {
        // same thing as above but were reading from the file
        language = File.ReadAllText("./settings.txt") == "1" ? Language.ENGLISH : Language.CROATIAN;
    }
}