﻿using System;

namespace BLREdit.API.REST_API.GitHub;

public sealed class GitHubAsset
{
    public string url { get; set; }
    public int id { get; set; }
    public string node_id { get; set; }
    public string name { get; set; }
    public object label { get; set; }
    public GitHubUploader uploader { get; set; }
    public string content_type { get; set; }
    public string state { get; set; }
    public int size { get; set; }
    public int download_count { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public string browser_download_url { get; set; }
}

