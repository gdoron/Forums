using System;

namespace Entities
{
    [Flags]
    public enum PostType
    {
        Torah = 0,
        News = 1,
        Sport = 2,
        Terror = 4,
        Picture = 8,
        Video = 16,
        Help = 32,
        OpinionArticle = 64,
        Foreign = 128,
    }
}