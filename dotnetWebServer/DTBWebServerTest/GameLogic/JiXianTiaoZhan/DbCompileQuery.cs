using DTBWebServer.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DTBWebServer.GameLogic.JiXianTiaoZhan
{
    public class JiXianTiaoZhanJoinDbUser
    {
        public DbJiXianTiaoZhanInfo mJiXianTiaoZhanInfo;
        public DbUser mDbUser;

        public JiXianTiaoZhanJoinDbUser(DbJiXianTiaoZhanInfo mJiXianTiaoZhanInfo, DbUser mDbUser)
        {
            this.mJiXianTiaoZhanInfo = mJiXianTiaoZhanInfo;
            this.mDbUser = mDbUser;
        }
    }

    public static class JiXianTiaoZhanDbCompileQuery
    {
        public static Func<WorldDBContext, int, IAsyncEnumerable<JiXianTiaoZhanJoinDbUser>> GetActivityUserList =
            EF.CompileAsyncQuery<WorldDBContext, int, JiXianTiaoZhanJoinDbUser>((WorldDBContext mDbContex, int nActivityId) =>
                mDbContex.JiXianTiaoZhanInfoList.Where((x) => x.nActivityId == nActivityId).Join(
                    mDbContex.t_user,
                    entryPoint => entryPoint.nUserId,
                    entry => entry.open_id,
                    (entryPoint, entry) => new JiXianTiaoZhanJoinDbUser(entryPoint, entry)
                 )
            );
    }

}
