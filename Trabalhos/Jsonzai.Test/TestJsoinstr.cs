using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Jsonzai.Test.Model;
using System.Globalization;
using Jsonzai.Instr;
using Newtonsoft.Json.Linq;

namespace Jsonzai.Test
{
    [TestClass]
    public class TestJsoninstr
    {
        [TestMethod]
        public void TestJsoninstrStudent()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = Jsoninstr.ToJson(expected);
            //string json = "{\"nr\":27721,\"name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJsoninstrArrayPrimitives()
        {
            int[] expected = { 4, 5, 6, 7 };
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = Jsoninstr.ToJson(expected);
            //string json = "[4,5,6,7]";
            int[] actual = JsonConvert.DeserializeObject<int[]>(json);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJsoninstrArrayDoublesPrimitives()
        {
            double[] expected = { 8.1, 4.9, 6.3, 7.7 };
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = Jsoninstr.ToJson(expected);
            //string json = "[4,5,6,7]";
            double[] actual = JsonConvert.DeserializeObject<double[]>(json);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestJsoninstrCourse()
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
            string json = Jsoninstr.ToJson(expected);
            /* string json = "{" +
                 "\"name\":\"AVE\"," +
                 "\"stds\":" +
                     "[" +
                         "{\"nr\":27721,\"name\":\"Ze Manel\"}," +
                         "{\"nr\":15642,\"name\":\"Maria Papoila\"}," +
                         "null," +
                         "null" +
                     "]" +
                 "}";*/
            Course actual = JsonConvert.DeserializeObject<Course>(json);
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        public void TestJsoninstrStudentFilteringByFields()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = null; //TODO: Jsoninstr.ToJson(expected, "f");
            //string json = "{\"nr\":27721,\"name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        public void TestJsoninstrStudentFilteringByProperties()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             */
            string json = null; //TODO: Jsoninstr.ToJson(expected, "p");
            //string json = "{\"Nr\":27721,\"Name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        public void TestJsoninstrStudentFilteringByMethods()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             * ToJson remove get_ dos nomes dos métodos
             */
            string json = null; //TODO: Jsoninstr.ToJson(expected, "m");
            //string json = "{\"Nr\":27721,\"Name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);
            Assert.AreEqual(expected, actual);
        }

        //[TestMethod]
        public void TestJsoninstrStudentFilteringByMethodsAndFields()
        {
            Student expected = new Student(27721, "Ze Manel");
            /*
             * O resultado de ToJson(expected) deve ser igual à string json abaixo
             * ToJson remove get_ dos nomes dos métodos
             */
            string json = null; //TODO: Jsoninstr.ToJson(expected, "mf");
            //string json = "{\"Nr\":27721,\"Name\":\"Ze Manel\",\"nr\":27721,\"name\":\"Ze Manel\"}";
            Student actual = JsonConvert.DeserializeObject<Student>(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]

        public void TestNull()
        {
            String expected = "null";
            System.Diagnostics.Debug.Write(typeof(int));
            string json = Jsoninstr.ToJson(null);
            Assert.AreEqual(expected, json);
        }

        [TestMethod]

        public void TestString()
        {
            String initial = "Demo";
            string json = Jsoninstr.ToJson(initial);
            String expected = "\"Demo\"";
            Assert.AreEqual(expected,json);
        }

        [TestMethod]

        public void TestChar()
        {
            char c = 'c';
            String json = Jsoninstr.ToJson(c);
            Assert.AreEqual("\"c\"", json);
        }

        [TestMethod]

        public void TestDateTime()
        {
            DateTime expected = new DateTime();

            String json = Jsoninstr.ToJson(expected);
            
            DateTime actual = JsonConvert.DeserializeObject<DateTime>(json);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]

        public void TestFloatingPoints()
        {
            double d = 2.1;
            String jsonDouble = Jsoninstr.ToJson(d);
            double actualDouble = JsonConvert.DeserializeObject<double>(jsonDouble);
            Assert.AreEqual(d, actualDouble);

            float single = 5.1F;
            String jsonSingle = Jsoninstr.ToJson(single);
            float actualSingle = JsonConvert.DeserializeObject<float>(jsonSingle);
            Assert.AreEqual(single, actualSingle);
        }

        [TestMethod]

        public void TestNumeric()
        {
            int i = int.MinValue;
            String jsonInt = Jsoninstr.ToJson(i);
            int actualInt = JsonConvert.DeserializeObject<int>(jsonInt);
            Assert.AreEqual(i, actualInt);

            long l = long.MaxValue;
            String jsonLong = Jsoninstr.ToJson(l);
            long actualLong = JsonConvert.DeserializeObject<long>(jsonLong);
            Assert.AreEqual(l, actualLong);

            UInt16 unsignedInt = UInt16.MinValue;
            String jsonUnsignedInt = Jsoninstr.ToJson(unsignedInt);
            UInt16 actualUnsignedInt = JsonConvert.DeserializeObject<UInt16>(jsonUnsignedInt);
            Assert.AreEqual(unsignedInt, actualUnsignedInt);
        }
    }
}
