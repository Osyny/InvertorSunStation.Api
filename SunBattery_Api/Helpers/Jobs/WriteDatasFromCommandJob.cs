using Quartz.Impl;
using Quartz;

namespace SunBattery_Api.Helpers.Jobs
{
    public static class WriteDatasFromCommandJob
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<WriteDatasJob>().Build();

         //   var startAtDateTime = DateTime.Today.AddHours(23);

            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                 //.StartAt(new DateTimeOffset(startAtDateTime))
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInSeconds(24)          // інтервал
                    .RepeatForever())                   // бесконечное повторение
                .Build();                               // создаем триггер

            await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
        }
    }
}
