using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Threading;
using System.Threading.Tasks;

namespace TPLDataflow
{
	public class Examples
	{
		public static void Example1()
		{
			var conf = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 4 };

			ActionBlock<int> a = new ActionBlock<int>(i =>
			{
				Thread.Sleep(500);
				Console.WriteLine(i);
			}, conf);
			TransformBlock<int, int> t = new TransformBlock<int, int>(i => i * 3);

			t.LinkTo(a);

            for (int i = 0; i < 12; i++)
            {
                t.Post(i);
            }
		}

		public static void Example2()
		{
            // Scenario: one buffer and 3 actions connected to it - each number will be processed only by one action
            var conf = new ExecutionDataflowBlockOptions();
            // todo: play with those
            conf.BoundedCapacity = 1;
            conf.MaxDegreeOfParallelism = 4;

			var buffer = new BufferBlock<int>();

			var action1 = new ActionBlock<int>(a => {
				Thread.Sleep(50);
				Console.WriteLine("Action 1 value: {0}", a);
			}, conf);

			var action2 = new ActionBlock<int>(a =>
			{
				Thread.Sleep(50);
				Console.WriteLine("Action 2 value: {0}", a);
            }, conf);

			var action3 = new ActionBlock<int>(a =>
			{
				Thread.Sleep(50);
				Console.WriteLine("Action 3 value: {0}", a);
            }, conf);

			buffer.LinkTo(action1);
			buffer.LinkTo(action2);
			buffer.LinkTo(action3);

			var t = new Task(() => {
				for (int i = 0; i < 12; i++)
				{
					buffer.Post(i);
				}
			});

            t.Start();
		}

        public static void Example3()
        {
            // Scenario: one buffer and 3 actions connected to it - each number precessed is send to each action
            var conf = new ExecutionDataflowBlockOptions();
            //conf.MaxDegreeOfParallelism = 4;

            var buffer = new BroadcastBlock<int>(i => i);

            var action1 = new ActionBlock<int>(a =>
            {
                Thread.Sleep(50);
                Console.WriteLine("Action 1 value: {0}", a);
            }, conf);

            var action2 = new ActionBlock<int>(a =>
            {
                Thread.Sleep(50);
                Console.WriteLine("Action 2 value: {0}", a);
            }, conf);

            var action3 = new ActionBlock<int>(a =>
            {
                Thread.Sleep(50);
                Console.WriteLine("Action 3 value: {0}", a);
            }, conf);

            buffer.LinkTo(action1);
            buffer.LinkTo(action2);
            buffer.LinkTo(action3);

            var t = new Task(() =>
            {
                for (int i = 0; i < 12; i++)
                {
                    buffer.Post(i);
                }
            });

            t.Start();
        }
	}
}
