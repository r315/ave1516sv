using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Jsonzai.Test.Model;
using System.Globalization;
using Jsonzai.Reflect;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Jsonzai.Test
{ 
    [TestClass]
    public class TestJsonfier
    {
        [TestMethod]
        public void TestToJson()
        {            
            ArrayList list = new ArrayList();
         /*   list.Add((object)10);
            list.Add((object)true);            
            list.Add((object) 'E');
            list.Add((object)"String");*/
            list.Add((object)new int[] { 0,1, 2, 3, 4 });
            //list.Add(new Student(27721,null));
           // list.Add(new DateTime());

            foreach (object test in list)
            {
                String expected = JsonConvert.SerializeObject((object)test);
                String actual = Jsonfier.ToJson(test);
                System.Diagnostics.Debug.WriteLine(expected);
         
                Assert.AreEqual(expected, actual);
            }            
        }

        [TestMethod]
        public void TestJsonfierStudent()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = Jsonfier.ToJson(expected);
            //string json = "{\"nr\":27721,\"name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);            
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJsonfierArrayPrimitives()
        {
            int [] expected = { 4, 5, 6, 7};
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = Jsonfier.ToJson(expected);
            //string json = "[4,5,6,7]";
            int[] actual = JsonConvert.DeserializeObject<int[]>(json);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJsonfierCourse()
        {
            Course expected = new Course
            (
                "AVE",
                new Student[4]{
                    new Student(27721, "Ze Manel"),
                    new Student(15642, "Maria Papoila"),
                    null,
                    null
                }
            );
        
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */          
            string json = "{" +
                "\"name\":\"AVE\"," +
                "\"stds\":" +
                    "[" +
                        "{\"nr\":27721,\"name\":\"Ze Manel\"}," +
                        "{\"nr\":15642,\"name\":\"Maria Papoila\"}," +
                        "null," +
                        "null" +
                    "]" +
                "}";
            Course actual = JsonConvert.DeserializeObject<Course>(json);
            Assert.AreEqual(expected, actual);
        }
    }
}
