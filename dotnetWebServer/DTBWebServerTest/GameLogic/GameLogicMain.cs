using DTBWebServer.DataBase;
using DTBWebServer.GameLogic.JiXianTiaoZhan;
using System.Diagnostics;

namespace DTBWebServer.GameLogic
{
    public class GameLogicMain : Singleton<GameLogicMain>
    {
        public async void Init(WebApplication app)
        {
            JiXianTiaoZhanModel.Instance.Init();


            var logger = app.Logger;
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    WorldDBContext mWorldDBContext = services.GetRequiredService<WorldDBContext>();
                    Debug.Assert(mWorldDBContext != null, "mWorldDBContext == null");
                    await DbInitializer.Initialize(mWorldDBContext);
                    logger.LogInformation("GameLogicMain Init");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}

