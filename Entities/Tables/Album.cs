using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTunesSearcher.Entities.Tables
{
    [Table("albums")]
    public class Album
    {
        [Key]
        [Column("collection_id")]
        public long collectionId { get; set; }

        [Column("collection_name")]
        public string collectionName { get; set; }

        [Column("artist_id")]
        public long artistId { get; set; }

        [Column("artist_name")]
        public string artistName { get; set; }

        [Column("track_count")]
        public short trackCount { get; set; }

        [Column("release_date")]
        public DateTime releaseDate { get; set; }

        [Column("primary_genre_name")]
        public string primaryGenreName { get; set; }
    }
}
