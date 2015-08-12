using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSortBy
{
    [TestFixture]
    public class DynamicSortTest
    {
        private List<EmptyClass> randomValues = new List<EmptyClass>();

        public void Setup(int itemCount)
        {
            var rnd = new Random();
            for (var ii = 0; ii < itemCount; ii++)
            {
                randomValues.Add(new EmptyClass() { Value = rnd.Next() });
            }

            rnd = new Random();
            randomValues = randomValues.OrderBy(item => rnd.Next()).ToList();
        }

        [TestCase(100000,"asc")]
        [TestCase(100000, "desc")]            
        public void DynamicSort(int itemCount,string direction)
        {
            Setup(itemCount);

            var sw = Stopwatch.StartNew();

            if (direction == "asc")
            {
                randomValues = randomValues.OrderByDynamic("Value").ToList();
            }
            else
            {
                randomValues = randomValues.OrderByDescDynamic("Value").ToList();
            }

            sw.Stop();

            var sw1 = Stopwatch.StartNew();

            if (direction == "asc")
            {
                randomValues = randomValues.OrderBy(m => m.Value).ToList();
            }
            else
            {
                randomValues = randomValues.OrderByDescending(m => m.Value).ToList();
            }

            sw.Stop();

            //just check part of collection
            for (var ii = 0; ii <= itemCount-1; ii++)
            {
                if (direction == "asc")
                {
                    Assert.LessOrEqual(randomValues[ii].Value, randomValues[++ii].Value);
                }
                else
                {
                    Assert.GreaterOrEqual(randomValues[ii].Value, randomValues[++ii].Value);
                }
                
            }

            Assert.Pass(string.Format("Elapsed dyn: {0} ms, Elapsedregular: {1} ms", sw.ElapsedMilliseconds, sw1.ElapsedMilliseconds));

        }

      
        class EmptyClass
        {
            public int Value { get; set; }
        }
    }
}
