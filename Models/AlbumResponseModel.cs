using iTunesSearcher.Entities.Tables;
using System.Collections.Generic;

namespace iTunesSearcher.Models
{
    public class AlbumResponseModel
    {
        public int resultCount { get; set; }
        public List<Album> results { get; set; }
    }
}
