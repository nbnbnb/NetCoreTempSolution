using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class LinqToTerraTest
    {
        internal static void Run()
        {
            // 首先创建一个自定义的 IQueryable
            // 内部关联一个自定义的 IQueryProvider
            // 这个自定义的 IQueryProvider 执行 from it in source 语句时
            // 将会调用 CreateQuery 方法，“每次”返回一个 IQueryable
            // 此时，CreateQuery 方法将会调用自定义 IQueryable 的另一个重载
            // 传递 this 进行关联，同时还有一个 Expression
            TerraServerDataQueryable<Place> terraPlaces = new TerraServerDataQueryable<Place>();
            string[] places = { "Johannesburg", "Yachats", "Seattle" };

            var query = from place in terraPlaces
                        where place.Name == "Johannesburg" &&
                        place.Name.StartsWith("Johan") &&
                        places.Contains(place.Name)
                        //places.ToList().Contains(place.Name)
                        select place.PlaceType;

            foreach (PlaceType placeType in query)
                Console.WriteLine(placeType);
        }
    }
}
