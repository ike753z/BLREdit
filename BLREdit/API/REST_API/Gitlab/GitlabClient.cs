﻿using BLREdit.API.REST_API.GitHub;
using BLREdit.Game.Proxy;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BLREdit.API.REST_API.Gitlab;

public static class GitlabClient
{
    public static readonly RESTAPIClient Client = new RESTAPIClient(RepositoryProvider.Gitlab, "https://gitlab.com/api/v4/");

    public static async Task<GitlabRelease> GetLatestRelease(string owner, string repo)
    {
        return await Client.GetLatestRelease<GitlabRelease>(owner, repo);
    }

    public static async Task<GitlabRelease[]> GetReleases(string owner, string repo)
    {
        return await Client.GetReleases<GitlabRelease>(owner, repo);
    }

    public static async Task<GitlabFile> GetFile(string owner, string repo, string branch, string file)
    {
        return await Client.GetFile<GitlabFile>(owner, repo, branch, file);
    }
}