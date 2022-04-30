﻿using Imya.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.Models.ModTweaker
{

    public class TweakCollection : ITweakStorage
    {
        public Dictionary<String, Tweak> Tweaks { get; set; } = new();

        public TweakCollection()
        {

        }

        public void SetTweakValue(string Filename, string ExposeID, string NewValue)
        {
            AddOrGetTweak(Filename).SetTweakValue(ExposeID, NewValue);
        }

        public bool TryGetTweakValue(string Filename, string ExposeID, out string? Value)
        {
            Value = GetTweak(Filename)?.GetTweakValue(ExposeID);
            return Value is not null;
        }

        public Tweak? GetTweak(String Filename)
        {
            return Tweaks.SafeGet(Filename);
        }

        public Tweak AddOrGetTweak(String Filename)
        {
            return Tweaks.SafeAddOrGet(Filename);
        }

        public void Save(String FilenameBase)
        {
            //filter name to be on the safe side.
            FilenameBase = Path.GetFileName(FilenameBase);

            String s = JsonConvert.SerializeObject(Tweaks, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(File.Create($"{FilenameBase}.json")))
            {
                writer.Write(s);
            }
        }

        public static TweakCollection? LoadFromFile(String FilenameBase)
        {
            try
            {
                return LoadFromJson(File.ReadAllText($"{FilenameBase}.json"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads a Tweak Collection From the file with the specified ID. If there is no stored collection, it creates a new one.
        /// </summary>
        /// <param name="FilenameBase"></param>
        /// <returns>The tweak collection</returns>
        public static TweakCollection LoadOrCreate(String FilenameBase)
        {
            var coll = LoadFromFile(FilenameBase);
            if (coll is not null) return coll;

            return new TweakCollection();
        }

        public static TweakCollection? LoadFromJson(String JsonString)
        {
            try
            {
                var coll = new TweakCollection();
                var tweaks = JsonConvert.DeserializeObject<Dictionary<String, Tweak>>(JsonString);
                if (tweaks is null) return null;
                coll.Tweaks = tweaks;

                return coll;
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine("Could not load Tweak Collection");
                return null;
            }
        }
    }
}