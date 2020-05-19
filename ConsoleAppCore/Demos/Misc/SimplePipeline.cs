using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Misc
{
    class SimplePipeline
    {
        private static void Run()
        {
            //ABC origin = new ABC();
            //var tp = C(origin);
            //tp = B(tp);
            //tp = A(tp);
            //Console.WriteLine(tp);

            //Console.WriteLine("---");

            //Console.WriteLine(A(B(C(origin))));

            Use(next =>
            {
                return context =>
                {
                    context.Write("1");
                    return next.Invoke(context);
                };
            });

            Use(next =>
            {
                return context =>
                {
                    context.Write("2");
                    return next.Invoke(context);
                };
            });

            funcs.Reverse();

            Func<RequestDelegate, RequestDelegate> a_link = next =>
            {
                Console.WriteLine("Init a_link");
                return context =>
                {
                    context.Write("1");
                    return next.Invoke(context);
                };
            };

            Func<RequestDelegate, RequestDelegate> b_link = next =>
            {
                Console.WriteLine("Init b_link");
                return context =>
                {
                    context.Write("2");
                    return next.Invoke(context);
                };
            };

            RequestDelegate start = (context) =>
            {
                context.Write("end");
                return Task.CompletedTask;
            };

            RequestDelegate run = b_link(a_link(start));

            run.Invoke(new Context());

            RequestDelegate end = (context) =>
            {
                context.Write("end");
                return Task.CompletedTask;
            };

            foreach (var middleWare in funcs)
            {
                end = middleWare.Invoke(end);
            }

            //end.Invoke(new Context());

        }

        static ABC A(ABC origin)
        {
            Console.WriteLine("A");
            return origin;
        }

        static ABC B(ABC origin)
        {
            Console.WriteLine("B");
            return origin;
        }

        static ABC C(ABC origin)
        {
            Console.WriteLine("C");
            return origin;
        }

        static List<Func<RequestDelegate, RequestDelegate>> funcs = new List<Func<RequestDelegate, RequestDelegate>>();

        public static void Use(Func<RequestDelegate, RequestDelegate> middleWare)
        {
            funcs.Add(middleWare);
        }

        public class ABC
        {

        }

        public class Context
        {
            public void Write(string msg)
            {
                Console.WriteLine(msg);
            }
        }

        public delegate Task RequestDelegate(Context context);
    }
}
