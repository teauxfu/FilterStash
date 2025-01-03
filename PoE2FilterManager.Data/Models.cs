﻿using Microsoft.AspNetCore.Components;
using System.Security.Cryptography;
using System.Text;

namespace PoE2FilterManager.Data
{

    public record PackageRef(string Name, string Source);

    // using a record here since these values are coming from gh api directly
    public record PackageItem(string Name, string HtmlUrl, string DownloadUrl, string Sha, int Size, string B64Content);

    public class Package(string name, string source)
    {
        public string Name { get; init; } = name;
        public string Source { get; init; } = source;
        public string? ReadMeHtml { get; set; }
        public string? B64Content { get; set; }
        public List<PackageItem> Items { get; set; } = [];
        public DateTimeOffset? LastUpdated { get; set; } 
        public string? LastHash { get; set; }   
        public string Hash
        {
            get
            {
                string s = string.Join(string.Empty, Items.Select(p => p.Sha));
                var bytes = Encoding.UTF8.GetBytes(s);
                var hash = SHA256.HashData(bytes);
                StringBuilder hexString = new();
                foreach (byte b in hash)
                    hexString.AppendFormat("{0:x2}", b);
                return hexString.ToString();
            }
        }
    }
 
    public class PackageIndex
    {
        public string? CurrentlyInstalledPackage { get; set; }
        public string? PreferredBackground { get; set; }
        public Dictionary<string, Package> Packages { get; set; } = [];
    }


    public class PackageDisplay(Package package)
    {
        public Package Package { get; init; } = package;
        public bool Selected { get; set; }
    }
}