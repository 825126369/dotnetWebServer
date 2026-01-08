using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DTBWebServer.DataBase
{

    public class DbJiXianTiaoZhanInfo
    {
        [Key]
        [Required]
        public string nUserId { get; set; }
        public uint nActivityId { get; set; }
        public uint nTime { get; set; }
        public uint nScore { get; set; }
        public uint nRank { get; set; }

        public override string ToString()
        {
            return $"{nUserId},{nActivityId},{nTime}, {nScore}, {nRank}";
        }
    }

    public class DbUser
    {
        public int user_id { get; set; }
        [Key]
        public string open_id { get; set; }

        public string? nick { get; set; }
       
        public string? icon { get; set; }

        public override string ToString()
        {
            return $"{user_id},{nick},{icon}";
        }
    }

    public class DbHotSong
    {
        public int hot_id { get; set; }
        [Key]
        public int? music_id { get; set; }

        public int? num { get; set; }

        public int? week_num { get; set; }

        public override string ToString()
        {
            return $"{hot_id},{music_id},{week_num}";
        }
    }

    public class WorldDBContext : DbContext
    {
        public DbSet<DbUser> t_user { get; set; }
        public DbSet<DbJiXianTiaoZhanInfo> JiXianTiaoZhanInfoList { get; set; }
        public DbSet<DbHotSong> t_hot { get; set; }

        public WorldDBContext(DbContextOptions<WorldDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbJiXianTiaoZhanInfo>().ToTable("JiXianTiaoZhanInfoList");
            modelBuilder.Entity<DbUser>().ToTable("t_user");
            modelBuilder.Entity<DbHotSong>().ToTable("t_hot");
        }
    }

}