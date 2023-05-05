using System.Threading;
using System;

namespace OSLab7
{
  internal class Program
  {
    static void Main(string[] args)
    {
      int? resf = null;
      var taskf = new Thread(() => { resf = f(); });
      int? resg = null;
      var taskg = new Thread(() => { resg = g(1); });
      taskf.Start();
      taskg.Start();
      Console.WriteLine("Tasks started");
      var askToStop = true;
      var timeout = 5;
      int? res = null;
      while (!TaskEndedOrCanceled(taskf) || !TaskEndedOrCanceled(taskg))
      {
        if ((resf.HasValue && resf.Value == 0) || (resg.HasValue && resg.Value == 0))
        {
          res = 0;
          break;
        }
        Thread.Sleep(timeout * 1000);
        var responseIsValid = false;
        while (!responseIsValid && askToStop)
        {
          responseIsValid = true;
          Console.Write("Tasks are not ready yet. Write\n'a' to abort tasks,\n'b' to wait {0} seconds more,\n'c' to wait until completion\n>", timeout);
          var response = Console.ReadLine();
          switch (response)
          {
            case "a":
              taskf.Abort();
              taskg.Abort();
              break;
            case "b":
              continue;
            case "c":
              askToStop = false;
              break;
            default:
              responseIsValid = false;
              break;
          }
        }
      }
      if (TaskCanceled(taskf) || TaskCanceled(taskg))
        Console.WriteLine("Tasks were aborted");
      else
      {
        if (res == null)
        {
          res = resf * resg;
        }
        taskf.Abort();
        taskg.Abort();
        Console.WriteLine("Result: {0}", res);
      }
    }

    static int f(int option = -1)
    {
      switch (option)
      {
        case 0: return 0;
        case 1:
          for (var i = 0; i < 15; i++) Thread.Sleep(1000);
          return 1;
        default:
          while (true) { };
      }
    }
    static int g(int option = -1)
    {
      switch (option)
      {
        case 0: return 1;
        case 1:
          for (var i = 0; i < 15; i++) Thread.Sleep(1000);
          return 0;
        default:
          while (true) { };
      }
    }
    static bool TaskEndedOrCanceled(Thread task)
    {
      if (!task.IsAlive || (task.ThreadState & ThreadState.Aborted) == ThreadState.Aborted || (task.ThreadState & ThreadState.Stopped) == ThreadState.Stopped) return true;
      return false;
    }
    static bool TaskCanceled(Thread task)
    {
      if ((task.ThreadState & ThreadState.Aborted) == ThreadState.Aborted) return true;
      return false;
    }
  }
}