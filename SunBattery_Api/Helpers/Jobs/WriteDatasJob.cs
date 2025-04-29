using Quartz;


namespace SunBattery_Api.Helpers.Jobs
{
    public class WriteDatasJob : IJob
    {
      //  private readonly IParseCommand _parseCommand;

        public WriteDatasJob()
        {
          //  _parseCommand = parseCommand;
        }
        public async Task Execute(IJobExecutionContext context)
        {
           //using(var com = new ParseCommand())
           // {
           //     com.ParseCommandStr();
           //     //  await Task.Delay(20000);
           // }
        }
    }
}
